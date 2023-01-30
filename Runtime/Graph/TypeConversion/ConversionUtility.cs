using System;
using System.Collections;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal static class ConversionUtility
    {
        private static readonly ConversionHandler handler = new();

        static ConversionUtility()
        {
            List<Type> types = ReflectionUtilities.GetAllDerivedTypes<ConversionSchema>();
            for (var i = 0; i < types.Count; i++)
            {
                var schema = Activator.CreateInstance(types[i]) as ConversionSchema;
                schema.Handle(handler);
            }
        }

        public static void Handle<TFrom, TTo>(Func<TFrom, TTo> converter)
        {
            handler.Handle(converter);
        }

        public static bool CanConvert<TFrom, TTo>()
        {
            return CanConvert(typeof(TFrom), typeof(TTo));
        }

        public static bool CanConvert(Type fromType, Type toType)
        {
            bool result = handler.CanConvert(fromType, toType);
            if (result)
            {
                return true;
            }

            result = CanConvertByDefault(fromType, toType);
            return result;
        }

        public static TTo Convert<TFrom, TTo>(TFrom value)
        {
            Func<TFrom, TTo> converter = GetConverter<TFrom, TTo>();
            if (converter != null)
            {
                TTo result = converter(value);
                return result;
            }

            return default;
        }

        public static Func<TFrom, TTo> GetConverter<TFrom, TTo>()
        {
            Func<TFrom, TTo> converter = handler.GetConverter<TFrom, TTo>();
            if (converter != null)
            {
                return converter;
            }

            // TODO: Prevent (un)boxing for value types
            Func<object, object> dufaultConverter = CreateConverterByDefault<TFrom, TTo>();
            if (dufaultConverter != null)
            {
                return (value) => (TTo)dufaultConverter(value);
            }

            return null;
        }

        internal static bool CanConvertByDefault(Type fromType, Type toType)
        {
            if (fromType == Missing.TYPE || toType == Missing.TYPE)
            {
                return false;
            }

            if (fromType == toType)
            {
                return true;
            }

            bool isFromTypeList = IsListType(fromType);
            bool isToTypeList = IsListType(toType);

            if (isFromTypeList && isToTypeList)
            {
                Type[] fromTypeArguments = fromType.GenericTypeArguments;
                if (fromTypeArguments.Length != 1)
                {
                    return false;
                }

                Type[] toTypeArguments = toType.GenericTypeArguments;
                if (toTypeArguments.Length != 1)
                {
                    return false;
                }

                return toTypeArguments[0].IsAssignableFrom(fromTypeArguments[0]);
            }
            else
            {
                bool isAssignableFrom = toType.IsAssignableFrom(fromType);
                if (isAssignableFrom)
                {
                    return true;
                }
                else if (!isFromTypeList && isToTypeList)
                {
                    Type[] toTypeArguments = toType.GenericTypeArguments;
                    if (toTypeArguments.Length != 1)
                    {
                        return false;
                    }

                    return toTypeArguments[0].IsAssignableFrom(fromType);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <returns>A function that can convert provided first arg value from type to type</returns>
        internal static Func<object, object> CreateConverterByDefault<TFrom, TTo>()
        {
            Type fromType = typeof(TFrom);
            Type toType = typeof(TTo);

            // Normal assignment. Note that IList can be assigned from List.
            if (toType.IsAssignableFrom(fromType))
            {
                return (value) => value;
            }

            bool isFromTypeList = IsListType(fromType);
            bool isToTypeList = IsListType(toType);

            // Single => Single: Normal assignment
            if (!isFromTypeList && !isToTypeList)
            {
                // Directly return null, since we already checked.
                return null;
            }

            // List => Single: Cannot cast list of objects to a single object
            if (isFromTypeList && !isToTypeList)
            {
                return null;
            }

            // Single => List: Create a list from the single object
            if (!isFromTypeList && isToTypeList)
            {
                Type[] toTypeArguments = toType.GenericTypeArguments;
                if (toTypeArguments.Length != 1)
                {
                    return null;
                }

                if (!toTypeArguments[0].IsAssignableFrom(fromType))
                {
                    return null;
                }

                return (value) =>
                {
                    Type genericListType = typeof(List<>).MakeGenericType(toTypeArguments[0]);
                    var list = Activator.CreateInstance(genericListType) as IList;
                    if (value != null)
                    {
                        list.Add(value);
                    }
                    return list;
                };
            }

            // List => List: Cast each list element
            if (isFromTypeList && isToTypeList)
            {
                Type[] fromTypeArguments = fromType.GenericTypeArguments;
                if (fromTypeArguments.Length != 1)
                {
                    return null;
                }

                Type[] toTypeArguments = toType.GenericTypeArguments;
                if (toTypeArguments.Length != 1)
                {
                    return null;
                }

                if (!toTypeArguments[0].IsAssignableFrom(fromTypeArguments[0]))
                {
                    return null;
                }

                return (value) =>
                {
                    var fromList = value as IList;
                    Type genericListType = typeof(List<>).MakeGenericType(toTypeArguments[0]);
                    var toList = Activator.CreateInstance(genericListType) as IList;
                    for (var i = 0; i < fromList.Count; i++)
                    {
                        toList.Add(fromList[i]);
                    }
                    return toList;
                };
            }

            return null;
        }

        private static bool IsListType(Type type)
        {
            if (type.InstanceOfGenericInterface(typeof(IList<>)))
            {
                return true;
            }

            if (type.InstanceOfGenericInterface(typeof(IReadOnlyList<>)))
            {
                return true;
            }

            return false;
        }
    }
}
