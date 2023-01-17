namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class TestNode : Node
    {
        public Inport<int> input;
        public Outport<int> output;
    }

    [NodeCategoryForTests]
    public class TestProcessNode : ProcessNode
    {
        public Inport<int> input;
        public Outport<int> output;
    }
}
