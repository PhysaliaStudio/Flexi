namespace Physalia.AbilityFramework
{
    public abstract class FlowNode : Node
    {
        public abstract FlowNode Previous { get; }
        public abstract FlowNode Next { get; }

        public AbilityState Run()
        {
            EvaluateInports();
            return DoLogic();
        }

        private void EvaluateInports()
        {
            foreach (Inport inport in Inports)
            {
                foreach (Outport outport in inport.GetConnections())
                {
                    if (outport.Node is ValueNode valueNode)
                    {
                        valueNode.Evaluate();
                    }
                }
            }
        }

        protected virtual AbilityState DoLogic()
        {
            return AbilityState.RUNNING;
        }

        public AbilityState Resume(IResumeContext resumeContext)
        {
            return ResumeLogic(resumeContext);
        }

        protected virtual AbilityState ResumeLogic(IResumeContext resumeContext)
        {
            return AbilityState.RUNNING;
        }

        protected void PushSelf()
        {
            Flow?.Push(this);
        }

        protected AbilityState WaitAndChoice(IChoiceContext context)
        {
            Flow.System?.TriggerChoice(context);
            return AbilityState.PAUSE;
        }

        protected void EnqueueEvent(IEventContext eventContext)
        {
            Flow.System?.EnqueueEvent(eventContext);
        }
    }
}
