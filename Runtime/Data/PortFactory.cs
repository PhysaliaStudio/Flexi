using System;

namespace Physalia.AbilityFramework
{
    public static class PortFactory
    {
        public static void CreateInport<T>(Node node, string portName)
        {
            CreateInport(node, typeof(T), portName);
        }

        public static void CreateOutport<T>(Node node, string portName)
        {
            CreateOutport(node, typeof(T), portName);
        }

        public static void CreateInport(Node node, Type portType, string portName)
        {
            if (node.GetPort(portName) != null)
            {
                Logger.Error($"[{nameof(PortFactory)}] Node(type:{node.GetType().FullName}) already has the port named with {portName}");
                return;
            }

            Type inportType = typeof(Inport<>).MakeGenericType(portType);
            var inport = Activator.CreateInstance(inportType) as Inport;

            inport.node = node;
            inport.name = portName;
            node.AddInport(portName, inport);
        }

        public static void CreateOutport(Node node, Type portType, string portName)
        {
            if (node.GetPort(portName) != null)
            {
                Logger.Error($"[{nameof(PortFactory)}] Node(type:{node.GetType().FullName}) already has the port named with {portName}");
                return;
            }

            Type outportType = typeof(Outport<>).MakeGenericType(portType);
            var outport = Activator.CreateInstance(outportType) as Outport;

            outport.node = node;
            outport.name = portName;
            node.AddOutport(portName, outport);
        }
    }
}
