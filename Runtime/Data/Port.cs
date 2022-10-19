using System;
using System.Collections;
using System.Collections.Generic;

namespace Physalia.AbilityFramework
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
                    list.Add(value);
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
