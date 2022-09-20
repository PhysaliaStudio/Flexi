using System;
using System.Collections.Generic;
using System.Reflection;

namespace Physalia.AbilitySystem
{
    internal static class Extensions
    {
        internal static FieldInfo[] GetFieldsIncludeBasePrivate(this Type type)
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
