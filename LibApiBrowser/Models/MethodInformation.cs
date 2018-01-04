using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace LibApiBrowser.Models
{
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
}