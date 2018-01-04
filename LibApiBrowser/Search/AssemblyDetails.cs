using System.Collections.Generic;
using Semver;

namespace LibApiBrowser.Search
{
    public class AssemblyDetails
    {
        public string Name { get; set; }

        public SemVersion Version { get; set; }

        public IEnumerable<(string guid, string framework)> Frameworks { get; set; }
    }
}