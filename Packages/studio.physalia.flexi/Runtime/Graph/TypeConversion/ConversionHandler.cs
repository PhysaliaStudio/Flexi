using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public interface IConversionHandler
    {
        void Handle<TFrom, TTo>(Func<TFrom, TTo> converter);
    }

    internal class ConversionHandler : IConversionHandler
    {
        private struct ConversionQuery : IEquatable<ConversionQuery>
        {
            public readonly Type fromType;
            public readonly Type toType;

            public ConversionQuery(Type fromType, Type toType)
            {
                this.fromType = fromType;
                this.toType = toType;
            }

            public bool Equals(ConversionQuery other)
            {
                return fromType == other.fromType && toType == other.toType;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ConversionQuery))
                {
                    return false;
                }

                return Equals((ConversionQuery)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(fromType, toType);
            }
        }

        private readonly Dictionary<ConversionQuery, object> converterTable = new();
        private readonly Dictionary<ConversionQuery, Func<object, object>> converterBoxedTable = new();

        public void Handle<TFrom, TTo>(Func<TFrom, TTo> converter)
        {
            var query = new ConversionQuery(typeof(TFrom), typeof(TTo));
            bool success = converterTable.TryAdd(query, converter);
            if (!success)
            {
                Logger.Error($"[{nameof(ConversionHandler)}] Converter has already exists! ({typeof(TFrom).Name} => {typeof(TTo).Name})");
                return;
            }

            object converterBoxed(object value) => converter((TFrom)value);
            converterBoxedTable.Add(query, converterBoxed);
        }

        public bool CanConvert<TFrom, TTo>()
        {
            return CanConvert(typeof(TFrom), typeof(TTo));
        }

        public bool CanConvert(Type fromType, Type toType)
        {
            var query = new ConversionQuery(fromType, toType);
            bool result = converterTable.ContainsKey(query);
            return result;
        }

        public Func<TFrom, TTo> GetConverter<TFrom, TTo>()
        {
            var query = new ConversionQuery(typeof(TFrom), typeof(TTo));
            bool success = converterTable.TryGetValue(query, out object converter);
            if (success)
            {
                return converter as Func<TFrom, TTo>;
            }

            return null;
        }

        public Func<object, object> GetConverterBoxed(Type fromType, Type toType)
        {
            var query = new ConversionQuery(fromType, toType);
            bool success = converterBoxedTable.TryGetValue(query, out Func<object, object> converterBoxed);
            if (success)
            {
                return converterBoxed;
            }

            return null;
        }
    }
}
