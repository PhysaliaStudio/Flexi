using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class Outport : Port
    {
        private readonly List<Inport> inports = new();

        protected Outport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
        {

        }

        internal abstract object GetValueBoxed();

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

        internal abstract bool TryGetConvertedValue<TTo>(out TTo result);

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

        internal override object GetValueBoxed()
        {
            return value;
        }

        public T GetValue()
        {
            return value;
        }

        public void SetValue(T value)
        {
            this.value = value;
        }

        internal override bool TryGetConvertedValue<TTo>(out TTo result)
        {
            Func<T, TTo> converter = ConversionUtility.GetConverter<T, TTo>();
            if (converter != null)
            {
                result = converter(value);
                return true;
            }

            result = default;
            return false;
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

            bool success = inport.TryGetConvertedValue(out T result);
            if (success)
            {
                value = result;
                return;
            }

            value = default;
        }

        public static implicit operator T(Outport<T> outport) => outport.value;
    }
}
