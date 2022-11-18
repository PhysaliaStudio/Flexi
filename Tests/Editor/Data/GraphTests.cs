using NUnit.Framework;

namespace Physalia.AbilityFramework.Tests
{
    public class GraphTests
    {
        [NodeCategory("Built-in/[Test Custom]")]
        public class IntNode : Node
        {
            public Outport<int> output;
        }

        [NodeCategory("Built-in/[Test Custom]")]
        public class DamageNode : ProcessNode
        {
            public Inport<int> baseValue;
        }

        [Test]
        public void AddNode_TheLastIndexOfNodeIsTheNewNode()
        {
            var graph = new Graph();
            IntNode intNode = graph.AddNewNode<IntNode>();
            Assert.AreEqual(intNode, graph.Nodes[0]);
        }

        [Test]
        public void AddNode_TheEntryNodeListDoesNotChange()
        {
            var graph = new Graph();
            _ = graph.AddNewNode<IntNode>();
            Assert.AreEqual(0, graph.EntryNodes.Count);
        }

        [Test]
        public void RemoveNode_TheNodeIsRemovedFromTheNodeList()
        {
            var graph = new Graph();
            IntNode intNode = graph.AddNewNode<IntNode>();
            graph.RemoveNode(intNode);
            Assert.AreEqual(0, graph.Nodes.Count);
        }

        [Test]
        public void RemoveNode_AlsoDisconnectFromAllOtherNodes()
        {
            var graph = new Graph();
            DamageNode damageNode = graph.AddNewNode<DamageNode>();
            IntNode intNode = graph.AddNewNode<IntNode>();
            damageNode.baseValue.Connect(intNode.output);

            graph.RemoveNode(intNode);
            Assert.AreEqual(0, damageNode.baseValue.GetConnections().Count);
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
