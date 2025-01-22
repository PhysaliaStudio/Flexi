using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal class EmptyContext : IEventContext
    {
        public static EmptyContext Instance { get; } = new EmptyContext();

        // Empty Content
    }

    public abstract class EntryNode<TContainer, TEventContext> : EntryNode
        where TContainer : AbilityContainer
        where TEventContext : IEventContext
    {
        public TContainer Container => GetContainer<TContainer>();

        public sealed override Type ContextType => typeof(TEventContext);

        protected internal sealed override bool CanExecute(IEventContext contextBase)
        {
            if (contextBase != null && contextBase is TEventContext context)
            {
                return CanExecute(context);
            }

            return false;
        }

        public abstract bool CanExecute(TEventContext context);

        protected sealed override AbilityState DoLogic()
        {
            TEventContext context = Flow.EventContext is TEventContext eventContext ? eventContext : default;
            return OnExecute(context);
        }

        protected abstract AbilityState OnExecute(TEventContext context);
    }

    public abstract class EntryNode<TContainer> : EntryNode
        where TContainer : AbilityContainer
    {
        public TContainer Container => GetContainer<TContainer>();

        public sealed override Type ContextType => typeof(EmptyContext);

        protected internal sealed override bool CanExecute(IEventContext contextBase)
        {
            if (contextBase != null && contextBase is EmptyContext)
            {
                return true;
            }

            return false;
        }

        protected sealed override AbilityState DoLogic()
        {
            return OnExecute();
        }

        protected abstract AbilityState OnExecute();
    }

    public abstract class EntryNode : FlowNode
    {
        internal Outport<FlowNode> next;

        public abstract Type ContextType { get; }

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
