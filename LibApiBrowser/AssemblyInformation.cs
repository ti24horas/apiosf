using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Semver;

namespace LibApiBrowser
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
                    .SelectMany(a => Extensions.AllTypes((IEnumerable<PropertyDefinition>)a.Properties))

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

            this.Version = Semver.SemVersion.Parse(version.ConstructorArguments.FirstOrDefault().Value.ToString());
            this.DefinedTypes = dll.Types.Where(a => a.IsPublic).AllTypes().OfType<TypeDefinition>().ToList();
        }
    }

    public class ClassInformation
    {
        public string AssemblyId { get; set; }

        public string FullName { get; set; }

        public MethodInformation[] Methods { get; }

        public string Id => $"{this.AssemblyId}_{this.FullName}";
        public string ClassName { get; set; }

        public ClassInformation(TypeDefinition t, string assemblyId)
        {
            this.AssemblyId = assemblyId;
            this.FullName = t.FullName.Replace("/", "+");
            this.ClassName = t.Name;
            this.Methods = t.Methods.Public()
                .Select(c => new MethodInformation(c)).ToArray();
        }

        public ClassInformation()
        {

        }
    }

    public class MethodInformation
    {
        public MethodInformation(MethodDefinition m)
        {
            // TODO: make a hash of all parameters as return type and modifiers/access mode are not allowed to be changed when overloaded

            this.Name = m.Name;
            this.IsStatic = m.IsStatic;
            // TODO: when web rendering, it's necessary to only render the final part.
            this.ReturnType = m.ReturnType.FullName;
            this.Parameters = m.Parameters.Select(s => new MethodParameter()
            {
                Name = s.Name,
                IsGeneric = s.ParameterType.HasGenericParameters,
                ParameterType = s.ParameterType.FullName,
                Types = s.ParameterType.HasGenericParameters ? s.ParameterType.GenericParameters.Select(_ => _.FullName).ToArray() : new string[0]
            }).ToList();
        }

        public bool IsStatic { get; set; }

        public string ReturnType { get; set; }

        public string Name { get; set; }

        public List<MethodParameter> Parameters { get; set; }
    }

    public class MethodParameter
    {
        public bool IsGeneric { get; set; }

        public string ParameterType { get; set; }

        public string[] Types { get; set; }

        public string Name { get; set; }
    }
}