using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class RecastContext : IResumeContext { }

    [NodeCategory("Action Game Sample")]
    public class WaitRecastNode : BaseProcessNode<DefaultAbilityContainer>
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

        protected override AbilityState OnExecute()
        {
            Container.Unit.AbilitySlot.SetToRecastState();
            return AbilityState.PAUSE;
        }

        public override bool CanResume(IResumeContext resumeContext)
        {
            return resumeContext is RecastContext;
        }

        protected override AbilityState OnResume(IResumeContext resumeContext)
        {
            if (resumeContext is RecastContext)
            {
                received = true;
                Container.Unit.AbilitySlot.SetToDisabledState();
                return AbilityState.RUNNING;
            }

            return AbilityState.PAUSE;
        }

        protected override AbilityState Tick()
        {
            currentTime += Time.deltaTime;
            if (currentTime * 1000 >= milliseconds.Value)
            {
                Container.Unit.AbilitySlot.SetToDisabledState();
                return AbilityState.RUNNING;
            }

            return AbilityState.PAUSE;
        }

        protected override void Reset()
        {
            currentTime = 0f;
            received = false;
        }
    }
}
