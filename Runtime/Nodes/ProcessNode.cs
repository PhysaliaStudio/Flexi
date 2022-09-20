using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public abstract class ProcessNode : Node
    {
        internal Inport<ProcessNode> previous;
        internal Outport<ProcessNode> next;

        public ProcessNode Previous
        {
            get
            {
                IReadOnlyList<Outport> connections = previous.GetConnections();
                return connections.Count > 0 ? connections[0].Node as ProcessNode : null;
            }
            set
            {
                previous.Connect(value.next);
            }
        }

        public ProcessNode Next
        {
            get
            {
                IReadOnlyList<Inport> connections = next.GetConnections();
                return connections.Count > 0 ? connections[0].Node as ProcessNode : null;
            }
            set
            {
                next.Connect(value.previous);
            }
        }
    }
}
