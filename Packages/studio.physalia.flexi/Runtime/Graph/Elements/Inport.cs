using System;
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

        internal abstract Func<TTo> GetValueConverter<TTo>();
    }

    public sealed class Inport<T> : Inport
    {
        private static readonly T globalDefaultValue;
        private T defaultValue;

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

            var convertFunc = outport.GetValueConverter<T>();
            if (convertFunc != null)
            {
                return convertFunc.Invoke();
            }

            return defaultValue;
        }

        internal override Func<TTo> GetValueConverter<TTo>()
        {
            Func<T, TTo> converter = ConversionUtility.GetConverter<T, TTo>();
            if (converter != null)
            {
                T value = GetValue();
                return () => converter(value);
            }

            return null;
        }

        public static implicit operator T(Inport<T> inport) => inport.GetValue();
    }
}
