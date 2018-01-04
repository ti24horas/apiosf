using Mono.Cecil;

namespace LibApiBrowser.Models
{
    public class PropertyInformation
    {
        public PropertyInformation(PropertyDefinition propDef)
        {
            // check if IsPublic assume protected members also.
            this.HasGetter = (propDef.GetMethod?.IsPublic).GetValueOrDefault(false) && (propDef.GetMethod?.IsGetter).GetValueOrDefault(false);

            this.HasSetter = (propDef.SetMethod?.IsPublic).GetValueOrDefault(false) && (propDef.SetMethod?.IsSetter).GetValueOrDefault(false);

            this.Type = propDef.PropertyType.FullName;

            this.Name = propDef.Name;
        }

        public string Type { get; }

        public bool HasSetter { get; }

        public bool HasGetter { get; }
        public string Name { get; set; }
    }
}