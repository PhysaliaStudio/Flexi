using System;

namespace Physalia.AbilitySystem
{
    public abstract class Port
    {
        internal Node node;
        internal string name;

        public Node Node => node;
        public string Name => name;
        public abstract Type ValueType { get; }

        protected abstract bool CanConnectTo(Port port);
        internal abstract void AddConnection(Port port);
        internal abstract void RemoveConnection(Port port);

        public void Connect(Port port)
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

        public void Disconnect(Port port)
        {
            RemoveConnection(port);
            port.RemoveConnection(this);
        }
    }
}
