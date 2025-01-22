namespace Physalia.Flexi
{
    [HideFromSearchWindow]
    internal class GraphOutputNode : BaseProcessNode
    {
        private static readonly int NODE_ID = -2;

        public Inport<FlowNode> previous;

        public GraphOutputNode()
        {
            id = NODE_ID;
        }

        public override FlowNode Next => null;
    }
}
