using System;
using System.Reflection;

namespace Physalia.AbilityFramework
{
    public static class PortFactory
    {
        public static Inport CreateInport<T>(Node node, string portName)
        {
            return CreateInportWithArgumentType(node, typeof(T), portName);
        }

        public static Outport CreateOutport<T>(Node node, string portName)
        {
            return CreateOutportWithArgumentType(node, typeof(T), portName);
        }

        public static Inport CreateInportWithArgumentType(Node node, Type portType, string portName)
        {
            Type inportType = typeof(Inport<>).MakeGenericType(portType);
            return CreateInportWithPortType(node, inportType, portName);
        }

        public static Outport CreateOutportWithArgumentType(Node node, Type portType, string portName)
        {
            Type outportType = typeof(Outport<>).MakeGenericType(portType);
            return CreateOutportWithPortType(node, outportType, portName);
        }

        public static Inport CreateInportWithPortType(Node node, Type portType, string portName)
        {
            if (node.GetPort(portName) != null)
            {
                Logger.Error($"[{nameof(PortFactory)}] Node(type:{node.GetType().FullName}) already has the port named with {portName}");
                return null;
            }

            var inport = Activator.CreateInstance(portType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { node, portName }, null) as Inport;
            node.AddInport(portName, inport);

            return inport;
        }

        public static Outport CreateOutportWithPortType(Node node, Type portType, string portName)
        {
            if (node.GetPort(portName) != null)
            {
                Logger.Error($"[{nameof(PortFactory)}] Node(type:{node.GetType().FullName}) already has the port named with {portName}");
                return null;
            }

            var outport = Activator.CreateInstance(portType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { node, portName }, null) as Outport;
            node.AddOutport(portName, outport);

            return outport;
        }
    }
}
