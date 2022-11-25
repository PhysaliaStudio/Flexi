using System;
using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public abstract class Outport : Port
    {
        private readonly List<Inport> inports = new();

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

        internal abstract Func<object> GetValueConverter(Type toType);
    }

    public sealed class Outport<T> : Outport
    {
        private T value;

        public override Type ValueType => typeof(T);

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
