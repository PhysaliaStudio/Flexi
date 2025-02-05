using System;
using System.Collections.Generic;

namespace Physalia.Flexi
{
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

        protected internal abstract bool CanExecute(TEventContext context);

        private protected sealed override FlowState ExecuteInternal()
        {
            TEventContext context = Flow.EventContext is TEventContext eventContext ? eventContext : default;
            return OnExecute(context);
        }

        protected abstract FlowState OnExecute(TEventContext context);
    }

    public abstract class EntryNode<TContainer, TEventContext, TResumeContext> : EntryNode<TContainer, TEventContext>
        where TContainer : AbilityContainer
        where TEventContext : IEventContext
        where TResumeContext : IResumeContext
    {
        internal sealed override bool CheckCanResume(IResumeContext resumeContext)
        {
            if (resumeContext != null && resumeContext is TResumeContext context)
            {
                return CanResume(context);
            }
            return false;
        }

        protected abstract bool CanResume(TResumeContext resumeContext);

        internal sealed override FlowState ResumeInternal(IResumeContext resumeContext)
        {
            TResumeContext context = resumeContext is TResumeContext resumeContextTyped ? resumeContextTyped : default;
            return OnResume(context);
        }

        protected abstract FlowState OnResume(TResumeContext resumeContext);
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
