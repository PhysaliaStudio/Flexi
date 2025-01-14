using System;
using System.Collections;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class Inport : Port
    {
        protected readonly List<Outport> outports = new();

        public object DefaultValue
        {
            get
            {
                return GetDefaultValueBoxed();
            }
            internal set
            {
                SetDefaultValueBoxed(value);
            }
        }

        protected Inport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
        {

        }

        public abstract bool IsDefaultValueSet();
        protected abstract object GetDefaultValueBoxed();
        protected abstract void SetDefaultValueBoxed(object value);

        protected override bool CanConnectTo(Port port)
        {
            return port is Outport;
        }

        protected override void AddConnection(Port port)
        {
            if (port is Outport outport)
            {
                outports.Add(outport);
            }
        }

        protected override void RemoveConnection(Port port)
        {
            if (port is Outport outport)
            {
                outports.Remove(outport);
            }
        }

        public override IReadOnlyList<Port> GetConnections()
        {
            return outports;
        }

        internal abstract bool TryGetConvertedValue<TTo>(out TTo result);
    }

    public class ListCache
    {
        public Type argumentType;
        public IList list;
    }

    public sealed class Inport<T> : Inport
    {
        private static readonly T globalDefaultValue;
        private T defaultValue;
        private readonly ListCache listCache;

        public override Type ValueType => typeof(T);
        public new T DefaultValue
        {
            get { return defaultValue; }
            internal set { defaultValue = value; }
        }

        static Inport()
        {
            globalDefaultValue = ConversionUtility.CreateDefaultInstance<T>();
        }

        internal Inport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
        {
            defaultValue = globalDefaultValue;

            bool isListType = ConversionUtility.IsListType(typeof(T));
            if (isListType)
            {
                listCache = new ListCache
                {
                    argumentType = typeof(T).GenericTypeArguments[0],
                    list = ConversionUtility.CreateDefaultInstance<T>() as IList
                };
            }
        }

        public override bool IsDefaultValueSet()
        {
            return !EqualityComparer<T>.Default.Equals(defaultValue, globalDefaultValue);
        }

        protected override object GetDefaultValueBoxed()
        {
            return defaultValue;
        }

        protected override void SetDefaultValueBoxed(object defaultValue)
        {
            this.defaultValue = (T)defaultValue;
        }

        public T GetValue()
        {
            if (outports.Count == 0)
            {
                return defaultValue;
            }

            T value = GetOutportValue(outports[0]);
            return value;
        }

        private T GetOutportValue(Outport outport)
        {
            if (outport is Outport<T> genericOutport)
            {
                T value = genericOutport.GetValue();
                return value;
            }

            // Perf: Cache list for optimize
            if (listCache != null && listCache.argumentType.IsAssignableFrom(outport.ValueType))
            {
                IList list = listCache.list;
                list.Clear();

                object value = outport.GetValueBoxed();
                if (value != null)  // TODO: I'm not sure if user want "null => [null]" or "null => []", I choose the latter.
                {
                    list.Add(value);
                }
                return (T)list;
            }

            bool success = outport.TryGetConvertedValue(out T result);
            if (success)
            {
                return result;
            }

            return defaultValue;
        }

        internal override bool TryGetConvertedValue<TTo>(out TTo result)
        {
            Func<T, TTo> converter = ConversionUtility.GetConverter<T, TTo>();
            if (converter != null)
            {
                T value = GetValue();
                result = converter(value);
                return true;
            }

            result = default;
            return false;
        }

        public static implicit operator T(Inport<T> inport) => inport.GetValue();
    }
}
