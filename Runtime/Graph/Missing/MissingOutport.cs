using System;

namespace Physalia.AbilityFramework
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

        internal override Func<object> GetValueConverter(Type toType)
        {
            return null;
        }

        internal override void SetValueFromInport(Inport inport)
        {

        }
    }
}
