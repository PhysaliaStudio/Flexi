using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class ProcessNode<TContainer> : ProcessNode
        where TContainer : AbilityDataContainer
    {
        public new TContainer Container
        {
            get
            {
                AbilityDataContainer baseContainer = base.Container;
                if (baseContainer == null)
                {
                    Logger.Error($"{GetType().Name}: container is null");
                    return null;
                }

                if (baseContainer is TContainer container)
                {
                    return container;
                }

                Logger.Error($"{GetType().Name}: Expect container is type: {typeof(TContainer).Name}, but is {baseContainer.GetType().Name}");
                return null;
            }
        }
    }

    public abstract class ProcessNode : FlowNode
    {
        internal Inport<FlowNode> previous;
        internal Outport<FlowNode> next;

        public override FlowNode Previous
        {
            get
            {
                IReadOnlyList<Port> connections = previous.GetConnections();
                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }

        public override FlowNode Next
        {
            get
            {
                IReadOnlyList<Port> connections = next.GetConnections();
                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }
    }
}
