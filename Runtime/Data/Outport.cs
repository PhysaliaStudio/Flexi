using System;
using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public abstract class Outport : Port
    {

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

        internal override void AddConnection(Port port)
        {
            if (port is Inport inport)
            {
                inports.Add(inport);
            }
        }

        internal override void RemoveConnection(Port port)
        {
            if (port is Inport inport)
            {
                inports.Remove(inport);
            }
        }

        internal IReadOnlyList<Inport> GetConnections()
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
    }
}
