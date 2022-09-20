using System;
using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public abstract class Outport : Port
    {
        internal abstract IReadOnlyList<Inport> GetConnections();
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

        internal override IReadOnlyList<Inport> GetConnections()
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
