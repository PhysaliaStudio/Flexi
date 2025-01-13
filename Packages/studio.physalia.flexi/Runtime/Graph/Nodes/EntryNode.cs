using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class EntryNode<T> : EntryNode where T : IEventContext
    {
        public override Type ContextType => typeof(T);

        public sealed override bool CanExecute(IEventContext contextBase)
        {
            if (contextBase != null && contextBase is T context)
            {
                return CanExecute(context);
            }

            return false;
        }

        public abstract bool CanExecute(T context);
    }

    public abstract class EntryNode : FlowNode
    {
        internal Outport<FlowNode> next;

        public virtual Type ContextType => null;

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

        public virtual bool CanExecute(IEventContext contextBase)
        {
            return false;
        }
    }
}
