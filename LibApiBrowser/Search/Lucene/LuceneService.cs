using System;
using System.Collections.Generic;
using System.Linq;
using LibApiBrowser.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace LibApiBrowser.Search.Lucene
{
    public class LuceneService
    {
        private IndexWriter writer;
        private string dir;

        private LuceneService(string dir)
        {
            this.dir = dir;
        }

        public IDisposable BeginWrite()
        {
            Analyzer a = new DottedAnalyzer();
            this.writer = new IndexWriter(FSDirectory.Open(this.dir), new IndexWriterConfig(LuceneVersion.LUCENE_48, a));
            return writer;
        }


        public void Flush()
        {
            this.writer.Commit();
            this.writer.Flush(true, true);
        }

        public enum DocumentType
        {
            Assembly,
            Class,
            Method,
            Property,
            Namespace,
            MethodParameter
        }

        public Guid AddAssembly(string assemblyName, string assemblyVersion, string framework = "net451")
        {
            var assemblyId = Guid.NewGuid();
            // add assembly document
            // TODO: this must be added only once
            // TODO: separate name and version
            this.writer.AddDocument(new Document
            {
                new StringField("_id", assemblyId.ToString(), Field.Store.YES),
                new StoredField("framework", framework),
                new TextField("name", assemblyName, Field.Store.YES),
                new StringField("version", assemblyVersion, Field.Store.YES),
                new StringField("type", "Assembly", Field.Store.YES),
            });

            return assemblyId;
        }

        public Guid AddClass(Guid assemblyId, ClassInformation cls)
        {
            var clsId = Guid.NewGuid();
            var ns = cls.FullName.Substring(0, cls.FullName.LastIndexOf('.') - 1);

            // adds the class to be searched
            this.writer.AddDocument(new Document
            {
                new StringField("_id", clsId.ToString(), Field.Store.YES),
                new StringField("_assemblyId", assemblyId.ToString(), Field.Store.NO),
                new TextField("name", cls.FullName, Field.Store.YES),
                new StringField("type", "Class", Field.Store.YES),
                new StoredField("clrType", cls.FullName),
                new StringField("namespace", ns, Field.Store.YES),
            });

            foreach (var m in cls.Methods)
            {
                this.AddMethod(clsId, m);
            }

            foreach (var p in cls.Properties)
            {
                this.AddProperty(clsId, p);
            }

            return clsId;
        }

        public void AddMethod(Guid classId, MethodInformation m)
        {
            var methodId = Guid.NewGuid();
            this.writer.AddDocument(new Document
            {
                new StringField("_id", methodId.ToString(), Field.Store.NO),
                new StringField("_classId", classId.ToString(), Field.Store.NO),
                new TextField("name", m.Name, Field.Store.YES),
                new TextField("returnType", m.ReturnType, Field.Store.YES),
                new StringField("type", "Method", Field.Store.YES),
            });

            int order = 0;
            foreach (var p in m.Parameters)
            {
                this.writer.AddDocument(new Document
                {
                    new StringField("_methodId", methodId.ToString(), Field.Store.YES),
                    new TextField("name", p.ParameterType, Field.Store.YES),
                    new StoredField("parameterOrder", order++),
                    new StoredField("clrType", p.ParameterType),
                    new Int32Field("type", (int) DocumentType.MethodParameter, Field.Store.YES),
                });
            }
        }

        public void AddProperty(Guid classId, PropertyInformation property)
        {
            var propertyId = Guid.NewGuid();
            this.writer.AddDocument(new Document
            {
                new StringField("_id", propertyId.ToString(), Field.Store.NO),
                new StringField("_classId", classId.ToString(), Field.Store.NO),
                new TextField("name", property.Name, Field.Store.YES),
                new TextField("propertyType", property.Type, Field.Store.YES),
                new StoredField("HasGetter", property.HasGetter.ToString()),
                new StoredField("HasSetter", property.HasGetter.ToString()),
                new StringField("type", "Property", Field.Store.YES)
            });
        }

        public static LuceneService Create(string dir)
        {
            return new LuceneService(dir);
        }

        public IEnumerable<Document> Search(string input, params DocumentType[] docTypes)
        {
            Analyzer a = new DottedAnalyzer();
            var p = new QueryParser(LuceneVersion.LUCENE_48, "name", a);

            return this.Search(p.Parse(input), docTypes);
        }

        public IEnumerable<Document> Search(Query query, params DocumentType[] docTypes)
        {
            using (var reader = DirectoryReader.Open(FSDirectory.Open(this.dir)))
            {
                var s = new IndexSearcher(reader);

                var b = new BooleanQuery { { query, Occur.SHOULD } };

                var orQ = new BooleanQuery();

                b.Add(orQ, Occur.MUST);

                foreach (var docType in docTypes)
                {
                    orQ.Add(new TermQuery(new Term("type", docType.ToString())), Occur.SHOULD);
                }

                var hitsFound = s.Search(query, 10);
                foreach (var r in hitsFound.ScoreDocs.OrderByDescending(_ => _.Score))
                {
                    yield return reader.Document(r.Doc);
                }
            }
        }
    }
}