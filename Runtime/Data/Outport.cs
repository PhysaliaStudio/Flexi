using System;
using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public abstract class Outport : Port
    {
        private readonly List<Inport> inports = new();

        protected Outport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
        {

        }

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

        /// <remarks>
        /// This method is used at the border nodes of macros:
        /// 1. From inports of MacroNodes to outports of GraphInputNodes
        /// 2. From inports of GraphOutputNodes to outports of MacroNodes
        /// </remarks>
        internal abstract void SetValueFromInport(Inport inport);
    }

    public sealed class Outport<T> : Outport
    {
        private T value;

        public override Type ValueType => typeof(T);

        internal Outport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
        {

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

        /// <remarks>
        /// This method is used at the border nodes of macros:
        /// 1. From inports of MacroNodes to outports of GraphInputNodes
        /// 2. From inports of GraphOutputNodes to outports of MacroNodes
        /// </remarks>
        internal override void SetValueFromInport(Inport inport)
        {
            if (inport is Inport<T> genericInport)
            {
                T value = genericInport.GetValue();
                this.value = value;
                return;
            }

            var convertFunc = inport.GetValueConverter(typeof(T));
            if (convertFunc != null)
            {
                T value = (T)convertFunc.Invoke();
                this.value = value;
                return;
            }

            value = default;
        }
    }
}
