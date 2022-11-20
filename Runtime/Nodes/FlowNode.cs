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

        public AbilityState Resume(INodeContext nodeContext)
        {
            return ResumeLogic(nodeContext);
        }

        protected virtual AbilityState ResumeLogic(INodeContext nodeContext)
        {
            return AbilityState.RUNNING;
        }

        protected void PushSelf()
        {
            Instance?.Push(this);
        }

        protected AbilityState WaitAndChoice(IChoiceContext context)
        {
            Instance.System?.TriggerChoice(context);
            return AbilityState.PAUSE;
        }
    }
}
