using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Semver;

namespace LibApiBrowser.Models
{
    public class AssemblyInformation
    {
        public string AssemblyName { get; set; }

        public SemVersion Version { get; set; }

        public IEnumerable<TypeDefinition> DefinedTypes { get; }

        public IEnumerable<AssemblyNameReference> AssemblyNameReferences
        {
            get
            {
                return this.DefinedTypes.AllTypes(true)
                    .SelectMany(a => a.Properties.AllTypes())

                    .Concat(this.DefinedTypes.SelectMany(a => a.Methods.Typed()))
                    .Where(a => a.Scope.MetadataScopeType == MetadataScopeType.AssemblyNameReference)
                    .Select(a => (AssemblyNameReference)a.Scope)
                    .GroupBy(a => a.Name + a.Version).Select(s => s.First());
            }
        }

        public AssemblyInformation(ModuleDefinition dll)
        {
            this.AssemblyName = dll.Assembly.FullName;
            var version = dll.Assembly.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == typeof(AssemblyInformationalVersionAttribute).FullName);

            if (version != null)
            {
                this.Version = SemVersion.Parse(version.ConstructorArguments.FirstOrDefault().Value.ToString());
            }

            this.DefinedTypes = dll.Types.Where(a => a.IsPublic).AllTypes().ToList();
        }
    }
}