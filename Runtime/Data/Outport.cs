using System;
using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public abstract class Outport : Port
    {
        internal abstract Func<object> GetValueConverter(Type toType);
    }

    public sealed class Outport<T> : Outport
    {
        private readonly List<Inport> inports = new();
        private T value;

        public override Type ValueType => typeof(T);

        protected override bool CanConnectTo(Port port)
        {
            return port is Inport;
        }

        protected override void AddConnection(Port port)
        {
            if (port is Inport inport)
            {
                inports.Add(inport);
            }
        }

        protected override void RemoveConnection(Port port)
        {
            if (port is Inport inport)
            {
                inports.Remove(inport);
            }
        }

        public override IReadOnlyList<Port> GetConnections()
        {
            return inports;
        }

        public T GetValue()
        {
            return value;
        }

        public void SetValue(T value)
        {
            this.value = value;
        }

        internal override Func<object> GetValueConverter(Type toType)
        {
            if (toType.IsAssignableFrom(ValueType))
            {
                return () => value;
            }

            var converter = GetConverter(ValueType, toType);
            if (converter != null)
            {
                return () => converter(value);
            }

            return null;
        }
    }
}
