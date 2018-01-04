using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LibApiBrowser.Models;
using LibApiBrowser.Search.Lucene;
using Mono.Cecil;
using Directory = System.IO.Directory;

namespace LibApiBrowser
{
    class Program
    {
        internal static IDictionary<string, string> Namemapper = new Dictionary<string, string>
        {
            [typeof(String).FullName] = "string",
            [typeof(void).FullName] = "void",
            [typeof(int).FullName] = "int",
            [typeof(long).FullName] = "long",
        };

        static void Main()
        {
            var svc = LuceneService.Create("p:\\lucene");

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
}
