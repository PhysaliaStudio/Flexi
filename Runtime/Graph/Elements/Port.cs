using System;
using System.Collections;
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

        protected static bool IsListType(Type type)
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
            if (outportType == Missing.TYPE || inportType == Missing.TYPE)
            {
                return false;
            }

            if (outportType == inportType)
            {
                return true;
            }

            bool isOutportList = IsListType(outportType);
            bool isInportList = IsListType(inportType);

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
            else
            {
                bool canInportAssignableFromOutport = inportType.IsAssignableFrom(outportType);
                if (canInportAssignableFromOutport)
                {
                    return true;
                }
                else if (!isOutportList && isInportList)
                {
                    Type[] inportListTypes = inportType.GenericTypeArguments;
                    if (inportListTypes.Length != 1)
                    {
                        return false;
                    }

                    return inportListTypes[0].IsAssignableFrom(outportType);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <returns>A function that can convert provided first arg value from type to type</returns>
        public static Func<object, object> GetConverter(Type outportType, Type inportType)
        {
            // Normal assignment
            if (inportType.IsAssignableFrom(outportType))
            {
                return (value) => value;
            }

            bool isOutportList = IsListType(outportType);
            bool isInportList = IsListType(inportType);

            if (!isOutportList && !isInportList)
            {
                if (inportType.IsAssignableFrom(outportType))
                {
                    return (value) => value;
                }

                return null;
            }

            if (isOutportList && !isInportList)  // Cannot cast list of objects to a single object
            {
                return null;
            }

            if (!isOutportList && isInportList)
            {
                Type[] inportListTypes = inportType.GenericTypeArguments;
                if (inportListTypes.Length != 1)
                {
                    return null;
                }

                if (!inportListTypes[0].IsAssignableFrom(outportType))
                {
                    return null;
                }

                return (value) =>
                {
                    Type genericListType = typeof(List<>).MakeGenericType(inportListTypes[0]);
                    var list = Activator.CreateInstance(genericListType) as IList;
                    if (value != null)
                    {
                        list.Add(value);
                    }
                    return list;
                };
            }

            if (isOutportList && isInportList)
            {
                Type[] outportListTypes = outportType.GenericTypeArguments;
                if (outportListTypes.Length != 1)
                {
                    return null;
                }

                Type[] inportListTypes = inportType.GenericTypeArguments;
                if (inportListTypes.Length != 1)
                {
                    return null;
                }

                if (!inportListTypes[0].IsAssignableFrom(outportListTypes[0]))
                {
                    return null;
                }

                return (value) =>
                {
                    var fromList = value as IList;
                    Type genericListType = typeof(List<>).MakeGenericType(inportListTypes[0]);
                    var toList = Activator.CreateInstance(genericListType) as IList;
                    for (var i = 0; i < fromList.Count; i++)
                    {
                        toList.Add(fromList[i]);
                    }
                    return toList;
                };
            }

            return null;
        }
    }
}
