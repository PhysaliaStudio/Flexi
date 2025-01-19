using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class ValueNode<TContainer> : ValueNode
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

    public abstract class ValueNode : Node
    {
        internal void Evaluate()
        {
            EvaluateInports();
            EvaluateSelf();
        }

        private void EvaluateInports()
        {
            IReadOnlyList<Inport> inports = Inports;
            for (var i = 0; i < inports.Count; i++)
            {
                Inport inport = inports[i];
                IReadOnlyList<Port> connections = inport.GetConnections();
                for (var j = 0; j < connections.Count; j++)
                {
                    var outport = connections[j] as Outport;
                    if (outport.Node is ValueNode valueNode)
                    {
                        valueNode.Evaluate();
                    }
                }
            }
        }

        protected abstract void EvaluateSelf();
    }
}
