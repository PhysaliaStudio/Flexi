using System;

namespace Physalia.AbilityFramework
{
    internal sealed class MissingInport : Inport, IIsMissing
    {
        public override Type ValueType => Missing.TYPE;

        internal MissingInport(Node node, string name)
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
