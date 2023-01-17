using System.Collections.Generic;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.FlowControl)]
    public class IfElseNode : FlowNode
    {
        internal Inport<FlowNode> previousPort;
        internal Inport<bool> conditionPort;
        internal Outport<FlowNode> truePort;
        internal Outport<FlowNode> falsePort;

        public override FlowNode Previous
        {
            get
            {
                IReadOnlyList<Port> connections = previousPort.GetConnections();
                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }

        public override FlowNode Next
        {
            get
            {
                IReadOnlyList<Port> connections;
                if (conditionPort.GetValue())
                {
                    connections = truePort.GetConnections();
                }
                else
                {
                    connections = falsePort.GetConnections();
                }

                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }
    }
}
