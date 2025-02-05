namespace Physalia.Flexi
{
    public abstract class BranchNode<TContainer> : BranchNode
        where TContainer : AbilityContainer
    {
        public TContainer Container => GetContainer<TContainer>();
    }

    public abstract class BranchNode<TContainer, TResumeContext> : BranchNode<TContainer>
        where TContainer : AbilityContainer
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

    public abstract class BranchNode : FlowNode
    {
        private protected sealed override FlowState ExecuteInternal()
        {
            return OnExecute();
        }

        protected virtual FlowState OnExecute()
        {
            return FlowState.Success;
        }
    }
}
