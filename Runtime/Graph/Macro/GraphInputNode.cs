using System.Collections.Generic;

namespace Physalia.Flexi
{
    [HideFromSearchWindow]
    internal class GraphInputNode : FlowNode
    {
        private static readonly int NODE_ID = -1;

        internal Outport<FlowNode> next;

        public GraphInputNode()
        {
            id = NODE_ID;
        }

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
