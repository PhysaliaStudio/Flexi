using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public class EmptyContext : IEventContext
    {
        public static EmptyContext Instance { get; } = new EmptyContext();

        // Empty Content
    }

    public abstract class EntryNode<T> : EntryNodeBase where T : IEventContext
    {
        public override Type ContextType => typeof(T);

        protected internal sealed override bool CanExecute(IEventContext contextBase)
        {
            if (contextBase != null && contextBase is T context)
            {
                return CanExecute(context);
            }

            return false;
        }

        public abstract bool CanExecute(T context);
    }

    public abstract class EntryNode : EntryNodeBase
    {
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
