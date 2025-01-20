using System;

namespace Physalia.Flexi
{
    internal sealed class MissingInport : Inport, IIsMissing
    {
        public override Type ValueType => Missing.TYPE;

        public MissingInport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
        {

        }

        public override bool IsDefaultValueSet()
        {
            return false;
        }

        protected override object GetDefaultValueBoxed()
        {
            return default;
        }

        protected override void SetDefaultValueBoxed(object value)
        {

        }

        protected override bool CanConnectTo(Port port)
        {
            return false;
        }

        internal override bool TryGetConvertedValue<TTo>(out TTo result)
        {
            result = default;
            return false;
        }
    }
}
