using System;
using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public abstract class Inport : Port
    {
        protected readonly List<Outport> outports = new();

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
    }

    public sealed class Inport<T> : Inport
    {
        public override Type ValueType => typeof(T);

        public T GetValue()
        {
            if (outports.Count == 0)
            {
                return default;
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

            var convertFunc = outport.GetValueConverter(typeof(T));
            if (convertFunc != null)
            {
                return (T)convertFunc.Invoke();
            }

            return default;
        }
    }
}
