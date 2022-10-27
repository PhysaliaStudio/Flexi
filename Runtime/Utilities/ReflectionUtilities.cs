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
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var list = new List<FieldInfo>();

            if (type.BaseType != null)
            {
                FieldInfo[] baseFields = type.BaseType.GetFieldsIncludeBasePrivate();
                list.AddRange(baseFields);
            }

            FieldInfo[] fields = type.GetFields(flags);
            list.AddRange(fields);
            return list.ToArray();
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
