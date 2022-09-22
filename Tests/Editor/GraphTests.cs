using NUnit.Framework;

namespace Physalia.AbilitySystem.Tests
{
    public class GraphTests
    {
        public class IntNode : Node
        {
            public Inport<int> baseValue;
        }

        [Test]
        public void AddNode_TheLastIndexOfNodeIsTheNewNode()
        {
            var graph = new Graph();
            IntNode intNode = graph.AddNode<IntNode>();
            Assert.AreEqual(intNode, graph.Nodes[0]);
        }

        [Test]
        public void AddNode_TheEntryNodeListDoesNotChange()
        {
            var graph = new Graph();
            _ = graph.AddNode<IntNode>();
            Assert.AreEqual(0, graph.EntryNodes.Count);
        }

        [Test]
        public void RemoveNode_TheNodeIsRemovedFromTheNodeList()
        {
            var graph = new Graph();
            IntNode intNode = graph.AddNode<IntNode>();
            graph.RemoveNode(intNode);
            Assert.AreEqual(0, graph.Nodes.Count);
        }

        [Test]
        public void AddEntryNode_TheLastIndexOfEntryNodeIsTheNewNode()
        {
            var graph = new Graph();
            StartNode startNode = graph.AddNode<StartNode>();
            Assert.AreEqual(startNode, graph.EntryNodes[0]);
        }

        [Test]
        public void RemoveEntryNode_TheEntryNodeIsRemovedFromTheEntryNodeList()
        {
            var graph = new Graph();
            StartNode startNode = graph.AddNode<StartNode>();
            graph.RemoveNode(startNode);
            Assert.AreEqual(0, graph.EntryNodes.Count);
        }
    }
}
