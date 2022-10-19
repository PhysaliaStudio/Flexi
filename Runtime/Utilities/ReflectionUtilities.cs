using System;
using System.Collections.Generic;
using System.Reflection;

namespace Physalia.AbilityFramework
{
    public static class ReflectionUtilities
    {
        private static Assembly[] assembliesCache;

        public static Assembly[] GetAssemblies()
        {
            if (assembliesCache == null)
            {
                assembliesCache = AppDomain.CurrentDomain.GetAssemblies();
            }

            return assembliesCache;
        }

        public static Type GetTypeByName(string typeName)
        {
            foreach (Assembly assembly in GetAssemblies())
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

        public static bool InstanceOfGenericInterface(this Type type, Type interfaceType)
        {
            if (type.InstanceOfGenericType(interfaceType))
            {
                return true;
            }

            Type[] interfaceTypes = type.GetInterfaces();
            for (var i = 0; i < interfaceTypes.Length; i++)
            {
                if (interfaceTypes[i].InstanceOfGenericType(interfaceType))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool InstanceOfGenericType(this Type type, Type genericType)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }
    }
}
