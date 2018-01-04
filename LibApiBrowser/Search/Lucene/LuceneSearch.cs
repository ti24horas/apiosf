using System.Collections.Generic;
using System.Linq;
using Semver;

namespace LibApiBrowser.Search.Lucene
{
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

            return Enumerable.GroupBy(result, s => new {s.name, s.version})
                .Select(s => new AssemblyDetails
                {
                    Version = s.Key.version,
                    Name = s.Key.name,
                    Frameworks = s.Select(_ => (_.id, _.framework))
                });
        }
    }
}