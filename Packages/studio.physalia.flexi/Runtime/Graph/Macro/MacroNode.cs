using System.Collections.Generic;

namespace Physalia.Flexi
{
    [HideFromSearchWindow]
    public sealed class MacroNode : BranchNode
    {
        private enum State { STANDBY, ENTERED, EXITED }

        public string key;

        internal Inport<FlowNode> previous;
        internal Outport<FlowNode> next;

        private AbilityGraph macroGraph;
        private State state;

        public override FlowNode Next
        {
            get
            {
                if (state == State.ENTERED)
                {
                    return macroGraph.GraphInputNode;
                }
                else if (state == State.EXITED)
                {
                    IReadOnlyList<Port> connections = next.GetConnections();
                    return connections.Count > 0 ? connections[0].Node as FlowNode : null;
                }
                else
                {
                    return null;
                }
            }
        }

        protected override FlowState OnExecute()
        {
            // If this node has already finished before, reset for restarting.
            // This may happen when looping.
            if (state == State.EXITED)
            {
                Reset();
            }

            if (state == State.STANDBY)
            {
                state = State.ENTERED;

                // Get graph
                macroGraph = Flow.Core.GetMacroGraph(key);
                for (var i = 0; i < macroGraph.Nodes.Count; i++)
                {
                    macroGraph.Nodes[i].flow = Flow;
                }

                // Copy inport values
                IReadOnlyList<Inport> inports = Inports;
                for (var i = 0; i < inports.Count; i++)
                {
                    Inport inport = inports[i];
                    if (inport == previous)
                    {
                        continue;
                    }

                    Outport macroOutport = macroGraph.GraphInputNode.GetOutport(inport.Name);
                    macroOutport.SetValueFromInport(inport);
                }

                // Push
                Flow.Graph.PushGraph(macroGraph);
            }
            else if (state == State.ENTERED)
            {
                state = State.EXITED;

                // Copy outport values
                IReadOnlyList<Outport> outports = Outports;
                for (var i = 0; i < outports.Count; i++)
                {
                    Outport outport = outports[i];
                    if (outport == next)
                    {
                        continue;
                    }

                    Inport macroInport = macroGraph.GraphOutputNode.GetInport(outport.Name);
                    outport.SetValueFromInport(macroInport);
                }
            }

            return FlowState.Success;
        }

        protected internal override void Reset()
        {
            macroGraph = null;
            state = State.STANDBY;
        }
    }
}
