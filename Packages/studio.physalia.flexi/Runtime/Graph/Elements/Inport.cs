using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class Inport : Port
    {
        protected readonly List<Outport> outports = new();

        protected Inport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
        {

        }

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
        private static T defaultValue;

        static Inport()
        {
            defaultValue = ConversionUtility.CreateDefaultInstance<T>();
        }

        public override Type ValueType => typeof(T);

        internal Inport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
        {

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
