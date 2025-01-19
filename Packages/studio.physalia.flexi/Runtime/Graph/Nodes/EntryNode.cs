using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal class EmptyContext : IEventContext
    {
        public static EmptyContext Instance { get; } = new EmptyContext();

        // Empty Content
    }

    public abstract class EntryNode<TContainer, TEventContext> : EntryNodeBase
        where TContainer : AbilityDataContainer
        where TEventContext : IEventContext
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

        public override Type ContextType => typeof(TEventContext);

        protected internal sealed override bool CanExecute(IEventContext contextBase)
        {
            if (contextBase != null && contextBase is TEventContext context)
            {
                return CanExecute(context);
            }

            return false;
        }

        public abstract bool CanExecute(TEventContext context);
    }

    public abstract class EntryNode<TContainer> : EntryNodeBase
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

        public sealed override Type ContextType => typeof(EmptyContext);

        protected internal sealed override bool CanExecute(IEventContext contextBase)
        {
            if (contextBase != null && contextBase is EmptyContext)
            {
                return true;
            }

            return false;
        }
    }

    public abstract class EntryNodeBase : FlowNode
    {
        internal Outport<FlowNode> next;

        public abstract Type ContextType { get; }

        public override FlowNode Previous => null;

        public override FlowNode Next
        {
            get
            {
                IReadOnlyList<Port> connections = next.GetConnections();
                return connections.Count > 0 ? connections[0].Node as FlowNode : null;
            }
        }

        internal bool CheckCanExecute(IEventContext contextBase)
        {
            EvaluateInports();
            return CanExecute(contextBase);
        }

        protected internal abstract bool CanExecute(IEventContext contextBase);
    }
}
