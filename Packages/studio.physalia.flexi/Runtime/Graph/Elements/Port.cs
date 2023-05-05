using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class Port
    {
        private readonly Node node;
        private readonly bool isDynamic;
        private string name;

        public Node Node => node;
        internal bool IsDynamic => isDynamic;
        public string Name { get { return name; } internal set { name = value; } }
        public abstract Type ValueType { get; }

        protected abstract bool CanConnectTo(Port port);
        protected abstract void AddConnection(Port port);
        protected abstract void RemoveConnection(Port port);
        public abstract IReadOnlyList<Port> GetConnections();

        protected Port(Node node, string name, bool isDynamic)
        {
            this.node = node;
            this.name = name;
            this.isDynamic = isDynamic;
        }

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

        internal void ConnectForce(Port port)
        {
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
