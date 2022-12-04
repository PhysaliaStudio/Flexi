namespace Physalia.AbilityFramework
{
    public sealed class SubgraphNode : ProcessNode
    {
        public string guid;

        public override FlowNode Previous => null;
        public override FlowNode Next => null;
    }
}
