using System;
using System.Collections.Generic;

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
        protected abstract void AddConnection(Port port);
        protected abstract void RemoveConnection(Port port);
        public abstract IReadOnlyList<Port> GetConnections();

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

        public void DisconnectAll()
        {
            IReadOnlyList<Port> connections = GetConnections();
            for (var i = connections.Count - 1; i >= 0; i--)
            {
                Disconnect(connections[i]);
            }
        }
    }
}
