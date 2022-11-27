using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    internal class GraphInputNode : FlowNode
    {
        internal Outport<FlowNode> next;

        public override FlowNode Previous => null;

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
