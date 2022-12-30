namespace Physalia.Flexi.Tests
{
    [NodeCategory("Built-in/[Test Custom]")]
    public class TestNode : Node
    {
        public Inport<int> input;
        public Outport<int> output;
    }

    [NodeCategory("Built-in/[Test Custom]")]
    public class TestProcessNode : ProcessNode
    {
        public Inport<int> input;
        public Outport<int> output;
    }
}
