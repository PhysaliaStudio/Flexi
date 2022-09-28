namespace Physalia.AbilitySystem
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

        public AbilityState Resume()
        {
            return ResumeLogic();
        }

        protected virtual AbilityState ResumeLogic()
        {
            return AbilityState.RUNNING;
        }
    }
}
