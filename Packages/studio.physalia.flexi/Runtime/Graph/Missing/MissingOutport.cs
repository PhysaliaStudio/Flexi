using System;

namespace Physalia.Flexi
{
    internal sealed class MissingOutport : Outport, IIsMissing
    {
        public override Type ValueType => Missing.TYPE;

        internal MissingOutport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
        {

        }

        protected override bool CanConnectTo(Port port)
        {
            return false;
        }

        internal override Func<TTo> GetValueConverter<TTo>()
        {
            return null;
        }

        internal override void SetValueFromInport(Inport inport)
        {

        }
    }
}
