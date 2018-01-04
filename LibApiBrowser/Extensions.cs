using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace LibApiBrowser
{
    internal static class Extensions
    {
        public static IEnumerable<MethodDefinition> Public(this IEnumerable<MethodDefinition> input)
        {
            return input.Where(a => a.IsPublic);
        }

        public static IEnumerable<MethodDefinition> NotFromProperty(this IEnumerable<MethodDefinition> input)
        {
            return input.Where(a => !a.IsGetter && !a.IsSetter);
        }

        public static IEnumerable<TypeReference> AllTypes(this TypeReference p)
        {
            yield return p;
            if (p.ContainsGenericParameter)
            {
                foreach (var parm in p.GenericParameters)
                {
                    yield return parm;
                }
            }
        }

        public static IEnumerable<TypeReference> AllTypes(this IEnumerable<PropertyDefinition> input)
        {
            foreach (var p in input.Where(a => a.GetMethod.IsPublic || a.SetMethod.IsPublic))
            {
                foreach (var a in p.PropertyType.AllTypes())
                {
                    yield return a;
                }
            }
        }

        public static IEnumerable<TypeReference> Typed(this IEnumerable<MethodDefinition> input)
        {
            foreach (var m in input)
            {
                foreach (var p in m.ReturnType.AllTypes())
                {
                    yield return p;
                }
                foreach (var p in m.Parameters)
                {
                    foreach (var t in p.ParameterType.AllTypes())
                    {
                        yield return t;
                    }
                }
            }
        }

        public static IEnumerable<TypeDefinition> AllTypes(this IEnumerable<TypeDefinition> t, bool includeExternalInterfacesAndBaseClasses = false)
        {
            foreach (var p in t.Where(a => a.IsPublic || a.IsNestedPublic))
            {
                yield return p;
                if (includeExternalInterfacesAndBaseClasses)
                {
                    foreach (var other in p.Interfaces.Select(a => a.InterfaceType).Concat(new[] { p.BaseType }).OfType<TypeDefinition>()
                        .AllTypes(true))
                    {
                        yield return other;
                    }
                }
                foreach (var nest in p.NestedTypes.AllTypes())
                {
                    yield return nest;
                }
            }

        }
    }
}