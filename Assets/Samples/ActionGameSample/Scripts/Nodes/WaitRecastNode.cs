using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class RecastContext : IResumeContext { }

    [NodeCategory("Action Game Sample")]
    public class WaitRecastNode : BranchNode<DefaultAbilityContainer, RecastContext>
    {
        public Inport<FlowNode> previous;
        public Outport<FlowNode> successNode;
        public Outport<FlowNode> timeoutNode;
        public Variable<int> milliseconds;

        private float currentTime;
        private bool received;

        public override FlowNode Next
        {
            get
            {
                if (received)
                {
                    IReadOnlyList<Port> connections = successNode.GetConnections();
                    return connections.Count > 0 ? connections[0].Node as FlowNode : null;
                }
                else
                {
                    IReadOnlyList<Port> connections = timeoutNode.GetConnections();
                    return connections.Count > 0 ? connections[0].Node as FlowNode : null;
                }
            }
        }

        protected override FlowState OnExecute()
        {
            Container.Unit.AbilitySlot.SetToRecastState();
            return FlowState.Pause;
        }

        protected override bool CanResume(RecastContext context)
        {
            return true;
        }

        protected override FlowState OnResume(RecastContext context)
        {
            received = true;
            Container.Unit.AbilitySlot.SetToDisabledState();
            return FlowState.Success;
        }

        protected override FlowState Tick()
        {
            currentTime += Time.deltaTime;
            if (currentTime * 1000 >= milliseconds.Value)
            {
                Container.Unit.AbilitySlot.SetToDisabledState();
                return FlowState.Success;
            }

            return FlowState.Pause;
        }

        protected override void Reset()
        {
            currentTime = 0f;
            received = false;
        }
    }
}
