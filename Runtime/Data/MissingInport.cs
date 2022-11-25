using System;

namespace Physalia.AbilityFramework
{
    internal sealed class MissingInport : Inport
    {
        public override Type ValueType => typeof(object);

        internal MissingInport(Node node, string name)
        {
            this.node = node;
            this.name = name;
        }

        protected override bool CanConnectTo(Port port)
        {
            return false;
        }
    }
}
