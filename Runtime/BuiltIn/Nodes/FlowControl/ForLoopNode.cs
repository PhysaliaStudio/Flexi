using System.Collections.Generic;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.FlowControl)]
    public class ForLoopNode : FlowNode
    {
        internal Inport<FlowNode> previousPort;
        internal Inport<int> startIndexPort;
        internal Inport<int> endIndexPort;

        internal Outport<FlowNode> loopBodyPort;
        internal Outport<int> indexPort;
        internal Outport<FlowNode> completedPort;

        private bool hasStarted;
        private bool isIncrement;
        private int index;

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
                if (isIncrement && index < endIndexPort.GetValue())
                {
                    connections = loopBodyPort.GetConnections();
                }
                else if (!isIncrement && index >= endIndexPort.GetValue())
                {
                    connections = loopBodyPort.GetConnections();
                }
                else
                {
                    connections = completedPort.GetConnections();
                }

                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }

        protected override AbilityState DoLogic()
        {
            if (!hasStarted)
            {
                hasStarted = true;
                int start = startIndexPort.GetValue();
                int end = endIndexPort.GetValue();

                isIncrement = end >= start;
                index = isIncrement ? start : start - 1;
            }
            else if (isIncrement && index < endIndexPort.GetValue())
            {
                index++;
            }
            else if (!isIncrement && index >= endIndexPort.GetValue())
            {
                index--;
            }

            // If we will enter the loop body, record this node
            if (isIncrement && index < endIndexPort.GetValue())
            {
                PushSelf();
            }
            else if (!isIncrement && index >= endIndexPort.GetValue())
            {
                PushSelf();
            }

            indexPort.SetValue(index);
            return AbilityState.RUNNING;
        }

        protected internal override void Reset()
        {
            hasStarted = false;
            isIncrement = false;
            index = 0;
        }
    }
}
