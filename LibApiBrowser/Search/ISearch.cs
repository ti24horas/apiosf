using System.Collections.Generic;
using Semver;

namespace LibApiBrowser.Search
{
    public interface ISearch
    {
        IEnumerable<AssemblyDetails> GetAssemblies(string framework = null, SemVersion version = null);
    }
}