using System;

namespace Physalia.AbilityFramework
{
    internal sealed class MissingOutport : Outport
    {
        public override Type ValueType => Missing.TYPE;

        internal MissingOutport(Node node, string name)
        {
            this.node = node;
            this.name = name;
        }

        protected override bool CanConnectTo(Port port)
        {
            return false;
        }

        internal override Func<object> GetValueConverter(Type toType)
        {
            return null;
        }
    }
}
