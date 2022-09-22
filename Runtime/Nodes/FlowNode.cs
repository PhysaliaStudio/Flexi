namespace Physalia.AbilitySystem
{
    public abstract class FlowNode : Node
    {
        public abstract FlowNode Previous { get; }
        public abstract FlowNode Next { get; }
    }
}
