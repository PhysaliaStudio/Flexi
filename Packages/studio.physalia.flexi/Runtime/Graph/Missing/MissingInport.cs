using System;

namespace Physalia.Flexi
{
    internal sealed class MissingInport : Inport, IIsMissing
    {
        public override Type ValueType => Missing.TYPE;

        internal MissingInport(Node node, string name, bool isDynamic) : base(node, name, isDynamic)
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
    }
}
