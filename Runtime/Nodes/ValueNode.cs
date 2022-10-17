namespace Physalia.AbilitySystem
{
    public abstract class ValueNode : Node
    {
        internal void Evaluate()
        {
            EvaluateInports();
            EvaluateSelf();
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

        protected abstract void EvaluateSelf();
    }
}
