using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    internal class GraphOutputNode : FlowNode
    {
        internal Inport<FlowNode> previous;

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
