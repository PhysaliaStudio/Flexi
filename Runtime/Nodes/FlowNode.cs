namespace Physalia.AbilitySystem
{
    public abstract class FlowNode : Node
    {
        public abstract FlowNode Previous { get; }
        public abstract FlowNode Next { get; }

        public void Run()
        {
            EvaluateInports();
            DoLogic();
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

        protected virtual void DoLogic() { }
    }
}
