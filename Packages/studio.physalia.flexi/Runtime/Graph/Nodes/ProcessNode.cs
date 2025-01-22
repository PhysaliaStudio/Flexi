using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class ProcessNode<TContainer> : ProcessNode
        where TContainer : AbilityContainer
    {
        public TContainer Container => GetContainer<TContainer>();
    }

    public abstract class BaseProcessNode<TContainer> : BaseProcessNode
        where TContainer : AbilityContainer
    {
        public TContainer Container => GetContainer<TContainer>();
    }

    public abstract class ProcessNode : BaseProcessNode
    {
        internal Inport<FlowNode> previous;
        internal Outport<FlowNode> next;

        public override FlowNode Next
        {
            get
            {
                IReadOnlyList<Port> connections = next.GetConnections();
                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }
    }

    public abstract class BaseProcessNode : FlowNode
    {
        private protected sealed override AbilityState DoLogic()
        {
            return OnExecute();
        }

        protected virtual AbilityState OnExecute()
        {
            return AbilityState.RUNNING;
        }
    }
}
