using System.Linq;
using Mono.Cecil;

namespace LibApiBrowser.Models
{
    public class ClassInformation
    {
        public string AssemblyId { get; set; }

        public string FullName { get; set; }

        public MethodInformation[] Methods { get; }

        public string Id => $"{this.AssemblyId}_{this.FullName}";

        public string ClassName { get; set; }

        public PropertyInformation[] Properties { get; set; }

        public ClassInformation(TypeDefinition t, string assemblyId)
        {
            this.AssemblyId = assemblyId;
            this.FullName = t.FullName.Replace("/", "+");
            this.ClassName = t.Name;
            this.Methods = t.Methods.Public()
                .Select(c => new MethodInformation(c)).ToArray();
            this.Properties = t.Properties.Where(a => (a.GetMethod != null && a.GetMethod.IsPublic && a.GetMethod.IsGetter) || (a.SetMethod != null && a.SetMethod.IsPublic && a.SetMethod.IsSetter)).Select(c => new PropertyInformation(c)).ToArray();

        }

        public ClassInformation()
        {

        }
    }
}