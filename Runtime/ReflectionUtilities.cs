using System;
using System.Collections.Generic;
using System.Reflection;

namespace Physalia.AbilitySystem
{
    public static class ReflectionUtilities
    {
        private static Assembly[] assembliesCache;

        public static Type GetTypeByName(string typeName)
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

        public static FieldInfo[] GetFieldsIncludeBasePrivate(this Type type)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var set = new HashSet<FieldInfo>();

            Type currentType = type;
            do
            {
                FieldInfo[] fields = type.GetFields(flags);
                for (var i = 0; i < fields.Length; i++)
                {
                    set.Add(fields[i]);
                }
            }
            while ((currentType = currentType.BaseType) != null);

            {
                var i = 0;
                var results = new FieldInfo[set.Count];
                foreach (FieldInfo field in set)
                {
                    results[i] = field;
                    i++;
                }
                return results;
            }
        }
    }
}
