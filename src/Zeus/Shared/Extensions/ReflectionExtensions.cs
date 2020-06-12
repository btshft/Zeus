using System.Reflection;

namespace Zeus.Shared.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool IsOverriden(this MethodInfo method)
        {
            return method.GetBaseDefinition().DeclaringType != method.DeclaringType;
        }
    }
}
