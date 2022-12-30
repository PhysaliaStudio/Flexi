using System.Collections.Generic;

namespace Physalia.Flexi
{
    [HideFromSearchWindow]
    internal class GraphOutputNode : FlowNode
    {
        private static readonly int NODE_ID = -2;

        internal Inport<FlowNode> previous;

        public GraphOutputNode()
        {
            id = NODE_ID;
        }

        public override FlowNode Previous
        {
            get
            {
                IReadOnlyList<Port> connections = previous.GetConnections();
                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }

        public override FlowNode Next => null;
    }
}
