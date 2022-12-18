using NUnit.Framework;

namespace Physalia.AbilityFramework.Tests
{
    public class GraphTests
    {
        [Test]
        public void AddNode_TheLastIndexOfNodeIsTheNewNode()
        {
            var graph = new Graph();
            TestNode node = graph.AddNewNode<TestNode>();
            Assert.AreEqual(node, graph.Nodes[0]);
        }

        [Test]
        public void AddNode_TheEntryNodeListDoesNotChange()
        {
            var graph = new Graph();
            _ = graph.AddNewNode<TestNode>();
            Assert.AreEqual(0, graph.EntryNodes.Count);
        }

        [Test]
        public void RemoveNode_TheNodeIsRemovedFromTheNodeList()
        {
            var graph = new Graph();
            TestNode node = graph.AddNewNode<TestNode>();
            graph.RemoveNode(node);
            Assert.AreEqual(0, graph.Nodes.Count);
        }

        [Test]
        public void RemoveNode_AlsoDisconnectFromAllOtherNodes()
        {
            var graph = new Graph();
            TestProcessNode node1 = graph.AddNewNode<TestProcessNode>();
            TestNode node2 = graph.AddNewNode<TestNode>();
            node1.input.Connect(node2.output);

            graph.RemoveNode(node2);
            Assert.AreEqual(0, node1.input.GetConnections().Count);
        }

        [Test]
        public void AddEntryNode_TheLastIndexOfEntryNodeIsTheNewNode()
        {
            var graph = new Graph();
            StartNode startNode = graph.AddNewNode<StartNode>();
            Assert.AreEqual(startNode, graph.EntryNodes[0]);
        }

        [Test]
        public void RemoveEntryNode_TheEntryNodeIsRemovedFromTheEntryNodeList()
        {
            var graph = new Graph();
            StartNode startNode = graph.AddNewNode<StartNode>();
            graph.RemoveNode(startNode);
            Assert.AreEqual(0, graph.EntryNodes.Count);
        }
    }
}
