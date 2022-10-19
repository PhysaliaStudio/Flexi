using NUnit.Framework;

namespace Physalia.AbilityFramework.Tests
{
    public class NodeFactoryTests
    {
        public class TestNode : Node
        {
            public Inport<int> input;
            public Outport<int> output;
        }

        public class TestProcessNode : ProcessNode
        {
            public Inport<int> input;
            public Outport<int> output;
        }

        [Test]
        public void Create_Normal()
        {
            TestNode node = NodeFactory.Create<TestNode>();

            Assert.AreEqual(true, node.input != null);
            Assert.AreEqual(true, node.output != null);

            Assert.AreEqual(node, node.input.Node);
            Assert.AreEqual(node, node.output.Node);

            Assert.AreEqual(nameof(node.input), node.input.Name);
            Assert.AreEqual(nameof(node.output), node.output.Name);

            Assert.AreEqual(node.input, node.GetInput(nameof(node.input)));
            Assert.AreEqual(node.output, node.GetOutput(nameof(node.output)));
        }

        [Test]
        public void Create_Process()
        {
            TestProcessNode node = NodeFactory.Create<TestProcessNode>();

            Assert.AreEqual(true, node.next != null);
            Assert.AreEqual(true, node.previous != null);
            Assert.AreEqual(true, node.input != null);
            Assert.AreEqual(true, node.output != null);

            Assert.AreEqual(node, node.next.Node);
            Assert.AreEqual(node, node.previous.Node);
            Assert.AreEqual(node, node.input.Node);
            Assert.AreEqual(node, node.output.Node);

            Assert.AreEqual(nameof(node.next), node.next.Name);
            Assert.AreEqual(nameof(node.previous), node.previous.Name);
            Assert.AreEqual(nameof(node.input), node.input.Name);
            Assert.AreEqual(nameof(node.output), node.output.Name);

            Assert.AreEqual(node.next, node.GetOutput(nameof(node.next)));
            Assert.AreEqual(node.previous, node.GetInput(nameof(node.previous)));
            Assert.AreEqual(node.input, node.GetInput(nameof(node.input)));
            Assert.AreEqual(node.output, node.GetOutput(nameof(node.output)));
        }
    }
}
