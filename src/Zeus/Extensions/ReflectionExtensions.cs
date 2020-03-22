using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Zeus.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool IsConcrete(this Type type)
        {
            if (type.IsSpecialName)
                return false;

            if (type.IsClass && !type.IsAbstract)
                return !type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true);

            return false;
        }

        public static bool IsAssignableTo<TOther>(this Type type) => type.IsAssignableTo(typeof(TOther));

        public static bool IsAssignableTo(this Type type, Type otherType)
        {
            return type.IsGenericTypeDefinition 
                ? type.IsAssignableToGenericTypeDefinition(otherType) 
                : otherType.IsAssignableFrom(type);
        }

        private static bool IsAssignableToGenericTypeDefinition(this Type type, Type genericType)
        {
            var typeInfo = type.GetTypeInfo();
            var genericTypeInfo = genericType.GetTypeInfo();

            var interfaceTypes = typeInfo.ImplementedInterfaces.Select(t => IntrospectionExtensions.GetTypeInfo(t));

            foreach (var interfaceType in interfaceTypes)
            {
                if (!interfaceType.IsGenericType) 
                    continue;

                var typeDefinitionTypeInfo = interfaceType
                    .GetGenericTypeDefinition()
                    .GetTypeInfo();

                if (typeDefinitionTypeInfo.Equals(genericTypeInfo))
                    return true;
            }

            if (typeInfo.IsGenericType)
            {
                var typeDefinitionTypeInfo = typeInfo
                    .GetGenericTypeDefinition()
                    .GetTypeInfo();

                if (typeDefinitionTypeInfo.Equals(genericTypeInfo))
                {
                    return true;
                }
            }

            var baseTypeInfo = typeInfo.BaseType?.GetTypeInfo();

            return !(baseTypeInfo is null) && baseTypeInfo.IsAssignableToGenericTypeDefinition(genericTypeInfo);
        }
    }
}