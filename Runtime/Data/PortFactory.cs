using System;

namespace Physalia.AbilityFramework
{
    public static class PortFactory
    {
        public static Inport CreateInport<T>(Node node, string portName)
        {
            return CreateInport(node, typeof(T), portName);
        }

        public static Outport CreateOutport<T>(Node node, string portName)
        {
            return CreateOutport(node, typeof(T), portName);
        }

        public static Inport CreateInport(Node node, Type portType, string portName)
        {
            if (node.GetPort(portName) != null)
            {
                Logger.Error($"[{nameof(PortFactory)}] Node(type:{node.GetType().FullName}) already has the port named with {portName}");
                return null;
            }

            Type inportType = typeof(Inport<>).MakeGenericType(portType);
            var inport = Activator.CreateInstance(inportType) as Inport;

            inport.node = node;
            inport.name = portName;
            node.AddInport(portName, inport);

            return inport;
        }

        public static Outport CreateOutport(Node node, Type portType, string portName)
        {
            if (node.GetPort(portName) != null)
            {
                Logger.Error($"[{nameof(PortFactory)}] Node(type:{node.GetType().FullName}) already has the port named with {portName}");
                return null;
            }

            Type outportType = typeof(Outport<>).MakeGenericType(portType);
            var outport = Activator.CreateInstance(outportType) as Outport;

            outport.node = node;
            outport.name = portName;
            node.AddOutport(portName, outport);

            return outport;
        }
    }
}
