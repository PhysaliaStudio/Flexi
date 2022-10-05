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

        private static bool IsListType(Type type)
        {
            if (type.InstanceOfGenericInterface(typeof(IList<>)))
            {
                return true;
            }

            if (type.InstanceOfGenericInterface(typeof(IReadOnlyList<>)))
            {
                return true;
            }

            return false;
        }

        public static bool CanPortCast(Type outportType, Type inportType)
        {
            if (outportType == inportType)
            {
                return true;
            }

            bool isOutportList = IsListType(outportType);
            bool isInportList = IsListType(inportType);

            if (!isOutportList && !isInportList)
            {
                return inportType.IsAssignableFrom(outportType);
            }

            if (isOutportList && !isInportList)  // Cannot cast list of objects to a single object
            {
                return false;
            }

            if (!isOutportList && isInportList)
            {
                Type[] inportListTypes = inportType.GenericTypeArguments;
                if (inportListTypes.Length != 1)
                {
                    return false;
                }

                return inportListTypes[0].IsAssignableFrom(outportType);
            }

            if (isOutportList && isInportList)
            {
                Type[] outportListTypes = outportType.GenericTypeArguments;
                if (outportListTypes.Length != 1)
                {
                    return false;
                }

                Type[] inportListTypes = inportType.GenericTypeArguments;
                if (inportListTypes.Length != 1)
                {
                    return false;
                }

                return inportListTypes[0].IsAssignableFrom(outportListTypes[0]);
            }

            return false;
        }
    }
}
