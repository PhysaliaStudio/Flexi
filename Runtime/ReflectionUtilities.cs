using System;
using System.Reflection;

namespace Physalia.AbilitySystem
{
    internal static class ReflectionUtilities
    {
        private static Assembly[] assembliesCache;

        internal static Type GetTypeByName(string typeName)
        {
            if (assembliesCache == null)
            {
                assembliesCache = AppDomain.CurrentDomain.GetAssemblies();
            }

            foreach (Assembly assembly in assembliesCache)
            {
                Type type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
