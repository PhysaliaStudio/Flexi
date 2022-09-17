using System;

namespace Physalia.AbilitySystem
{
    public abstract class Port
    {
        internal Node node;

        public Node Node => node;
        public abstract Type ValueType { get; }

        protected abstract bool CanConnectTo(Port port);
        protected abstract void AddConnection(Port port);

        public void ConnectTo(Port port)
        {
            if (!CanConnectTo(port))
            {
                return;
            }

            if (!port.CanConnectTo(this))
            {
                return;
            }

            AddConnection(port);
            port.AddConnection(this);
        }
    }
}
