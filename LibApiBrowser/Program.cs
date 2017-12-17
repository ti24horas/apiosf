using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Mono.Cecil;
using Semver;
using Directory = System.IO.Directory;
using MethodSemanticsAttributes = Mono.Cecil.MethodSemanticsAttributes;

namespace LibApiBrowser
{
    class Program
    {
        internal static IDictionary<string, string> namemapper = new Dictionary<string, string>
        {
            [typeof(String).FullName] = "string",
            [typeof(void).FullName] = "void",
            [typeof(int).FullName] = "int",
            [typeof(long).FullName] = "long",
        };

        static void Main(string[] args)
        {
            var svc = LuceneService.Create("p:\\lucene");
            TestDottedAnalyzer.Test();


            var dll = ModuleDefinition.ReadModule("p:\\ClassLibraryTest.dll");
            Console.WriteLine(dll.Runtime);
            var version = dll.Assembly.CustomAttributes
                .FirstOrDefault(a => a.AttributeType.FullName == typeof(AssemblyInformationalVersionAttribute).FullName)
                ?.ConstructorArguments.FirstOrDefault().Value;
            if (version != null)
            {
                Console.WriteLine($"Version: {version}");
            }

            var info = new AssemblyInformation(dll);

            foreach (var r in info.AssemblyNameReferences.Concat(dll.AssemblyReferences))
            {
                Console.WriteLine("{0} {1}", r.Name, r.Version);
            }
            Directory.Delete("p:\\lucene", true);
            using (svc.BeginWrite())
            {
                var guid = svc.AddAssembly(dll.Assembly.Name.Name, info.Version.ToString());

                foreach (var t in info.DefinedTypes.GroupBy(a => a.Namespace))
                {
                    Console.WriteLine(t.Key);

                    foreach (var type in t)
                    {
                        var cls = new ClassInformation(type, dll.Assembly.Name.Name + " " + version);
                        svc.AddClass(guid, cls);
                        Console.WriteLine("\t{0}", type.FullName);
                    }
                }
                svc.Flush();
            }
            while (true)
            {
                //Query q = new TermQuery(new Term("name", Console.ReadLine()));
                var results = svc.Search(Console.ReadLine(), LuceneService.DocumentType.Class, LuceneService.DocumentType.Assembly, LuceneService.DocumentType.Method).ToList();
            }
            Console.ReadLine();
        }
    }

    public class WebServer
    {
        public WebServer(ISearch searcher)
        {
            
        }
    }

    public class AssemblyDetails
    {
        public string Name { get; set; }

        public SemVersion Version { get; set; }

        public IEnumerable<(string guid, string framework)> Frameworks { get; set; }
    }

    public interface ISearch
    {
        IEnumerable<AssemblyDetails> GetAssemblies(string framework = null, SemVersion version = null);
    }

    public class LuceneSearch : ISearch
    {
        private readonly LuceneService _svc;

        public LuceneSearch(LuceneService svc)
        {
            _svc = svc;
        }

        public IEnumerable<AssemblyDetails> GetAssemblies(string framework = null, SemVersion version = null)
        {
            var result = from item in this._svc.Search("type:Assembly", LuceneService.DocumentType.Assembly)
                         select  new
                {
                    name = item.Get("name"),
                    version = SemVersion.Parse(item.Get("version")),
                    framework = item.Get("framework"),
                    id = item.Get("_id")
                };


            return result.GroupBy(s => new {s.name, s.version})
                .Select(s => new AssemblyDetails
                {
                    Version = s.Key.version,
                    Name = s.Key.name,
                    Frameworks = s.Select(_ => (_.id, _.framework))
                });
        }
    }
    
}
