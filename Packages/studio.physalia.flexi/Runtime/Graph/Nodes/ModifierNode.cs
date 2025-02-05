using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class ModifierNode<TContainer> : ModifierNode
        where TContainer : AbilityContainer
    {
        public TContainer Container => GetContainer<TContainer>();
    }

    public abstract class ModifierNode : FlowNode
    {
        internal Inport<FlowNode> previous;
        internal Outport<FlowNode> next;

        public sealed override bool ShouldTriggerChainEvents => false;

        public sealed override FlowNode Next
        {
            get
            {
                IReadOnlyList<Port> connections = next.GetConnections();
                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }

        private protected sealed override FlowState ExecuteInternal()
        {
            return OnExecute();
        }

        protected virtual FlowState OnExecute()
        {
            return FlowState.Success;
        }

        internal sealed override bool CheckCanResume(IResumeContext resumeContext)
        {
            return false;
        }

        protected internal sealed override FlowState Tick()
        {
            return FlowState.Success;
        }
    }
}
