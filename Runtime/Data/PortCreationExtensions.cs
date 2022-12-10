using System;
using System.Reflection;

namespace Physalia.AbilityFramework
{
    internal static class PortCreationExtensions
    {
        internal static Inport CreateInport<T>(this Node node, string portName)
        {
            return CreateInportWithArgumentType(node, typeof(T), portName);
        }

        internal static Outport CreateOutport<T>(this Node node, string portName)
        {
            return CreateOutportWithArgumentType(node, typeof(T), portName);
        }

        internal static Inport CreateInportWithArgumentType(this Node node, Type portType, string portName)
        {
            Type inportType = typeof(Inport<>).MakeGenericType(portType);
            return CreateInportWithPortType(node, inportType, portName);
        }

        internal static Outport CreateOutportWithArgumentType(this Node node, Type portType, string portName)
        {
            Type outportType = typeof(Outport<>).MakeGenericType(portType);
            return CreateOutportWithPortType(node, outportType, portName);
        }

        internal static Inport CreateInportWithPortType(this Node node, Type portType, string portName)
        {
            if (node.GetPort(portName) != null)
            {
                Logger.Error($"[Create Port] Node(type:{node.GetType().FullName}) already has the port named with {portName}");
                return null;
            }

            var inport = Activator.CreateInstance(portType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { node, portName }, null) as Inport;
            node.AddInport(portName, inport);

            return inport;
        }

        internal static Outport CreateOutportWithPortType(this Node node, Type portType, string portName)
        {
            if (node.GetPort(portName) != null)
            {
                Logger.Error($"[Create Port] Node(type:{node.GetType().FullName}) already has the port named with {portName}");
                return null;
            }

            var outport = Activator.CreateInstance(portType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { node, portName }, null) as Outport;
            node.AddOutport(portName, outport);

            return outport;
        }
    }
}
