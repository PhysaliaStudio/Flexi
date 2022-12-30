using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class ProcessNode : FlowNode
    {
        internal Inport<FlowNode> previous;
        internal Outport<FlowNode> next;

        public override FlowNode Previous
        {
            get
            {
                IReadOnlyList<Port> connections = previous.GetConnections();
                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }

        public override FlowNode Next
        {
            get
            {
                IReadOnlyList<Port> connections = next.GetConnections();
                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }
    }
}
