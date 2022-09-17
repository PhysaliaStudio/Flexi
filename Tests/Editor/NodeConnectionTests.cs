using NUnit.Framework;

namespace Physalia.AbilitySystem.Tests
{
    public class NodeConnectionTests
    {
        public class TestNode1 : Node
        {
            public Inport<int> input = new();
        }

        public class TestNode2 : Node
        {
            public Outport<int> output = new();
        }

        [Test]
        public void GetValue_FromEmptyInport()
        {
            TestNode1 node1 = new TestNode1();
            Assert.AreEqual(0, node1.input.GetValue());
        }

        [Test]
        public void GetValue_FromConnectedInport()
        {
            TestNode1 node1 = new TestNode1();
            TestNode2 node2 = new TestNode2();

            node1.input.ConnectTo(node2.output);
            node2.output.SetValue(5421);

            Assert.AreEqual(5421, node1.input.GetValue());
        }
    }
}
