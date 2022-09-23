using System;
using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public abstract class Inport : Port
    {

    }

    public sealed class Inport<T> : Inport
    {
        private readonly List<Outport> outports = new();
        private readonly List<T> valuesCache = new();

        public override Type ValueType => typeof(T);

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

        public T GetValue()
        {
            if (outports.Count == 0)
            {
                return default;
            }

            T value = GetOutportValue(outports[0]);
            return value;
        }

        public IReadOnlyList<T> GetValues()
        {
            for (var i = 0; i < outports.Count; i++)
            {
                T value = GetOutportValue(outports[i]);
                valuesCache.Add(value);
            }

            return valuesCache;
        }

        private T GetOutportValue(Outport outport)
        {
            if (outport is Outport<T> genericOutport)
            {
                T value = genericOutport.GetValue();
                return value;
            }

            return default;
        }
    }
}
