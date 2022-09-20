using Newtonsoft.Json;
using NUnit.Framework;
using Physalia.AbilitySystem.StatSystem;

namespace Physalia.AbilitySystem.Tests
{
    public class GraphConverterTests
    {
        public class DamageNode : ProcessNode
        {
            public Inport<StatOwner> owners;
            public Inport<int> baseValue;
        }

        public class OwnerFilterNode : Node
        {
            public Outport<StatOwner> owners;
        }

        public class IntNode : Node
        {
            public Outport<StatOwner> output;
            public Variable<int> value;
        }

        public class LogNode : ProcessNode
        {
            public Inport<string> text;
        }

        [Test]
        public void Serialize_Normal()
        {
            DamageNode damageNode = NodeFactory.Create<DamageNode>();
            OwnerFilterNode filterNode = NodeFactory.Create<OwnerFilterNode>();
            IntNode intNode = NodeFactory.Create<IntNode>();
            LogNode logNode = NodeFactory.Create<LogNode>();

            damageNode.Next = logNode;
            damageNode.owners.Connect(filterNode.owners);
            damageNode.baseValue.Connect(intNode.output);

            Graph graph = new Graph();
            graph.nodes.Add(damageNode);
            graph.nodes.Add(filterNode);
            graph.nodes.Add(intNode);
            graph.nodes.Add(logNode);

            graph.ReorderNodes();
            var expected =
                "{\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+DamageNode\"}," +
                "{\"_id\":2,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+OwnerFilterNode\"}," +
                "{\"_id\":3,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+IntNode\",\"value\":0}," +
                "{\"_id\":4,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+LogNode\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":4,\"port2\":\"previous\"}," +
                "{\"id1\":1,\"port1\":\"owners\",\"id2\":2,\"port2\":\"owners\"}," +
                "{\"id1\":1,\"port1\":\"baseValue\",\"id2\":3,\"port2\":\"output\"}]}";

            string json = JsonConvert.SerializeObject(graph);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void Deserialize_Normal()
        {
            var json =
                "{\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+DamageNode\"}," +
                "{\"_id\":2,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+OwnerFilterNode\"}," +
                "{\"_id\":3,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+IntNode\",\"value\":0}," +
                "{\"_id\":4,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+LogNode\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":4,\"port2\":\"previous\"}," +
                "{\"id1\":1,\"port1\":\"owners\",\"id2\":2,\"port2\":\"owners\"}," +
                "{\"id1\":1,\"port1\":\"baseValue\",\"id2\":3,\"port2\":\"output\"}]}";

            Graph graph = JsonConvert.DeserializeObject<Graph>(json);

            Assert.AreEqual(4, graph.nodes.Count);
            Assert.AreEqual(true, graph.nodes[0] is DamageNode);
            Assert.AreEqual(true, graph.nodes[1] is OwnerFilterNode);
            Assert.AreEqual(true, graph.nodes[2] is IntNode);
            Assert.AreEqual(true, graph.nodes[3] is LogNode);

            var damageNode = graph.nodes[0] as DamageNode;
            var filterNode = graph.nodes[1] as OwnerFilterNode;
            var intNode = graph.nodes[2] as IntNode;
            var logNode = graph.nodes[3] as LogNode;

            Assert.AreEqual(logNode, damageNode.Next);
            Assert.AreEqual(damageNode, logNode.Previous);

            Assert.AreEqual(filterNode.owners, damageNode.owners.GetConnections()[0]);
            Assert.AreEqual(intNode.output, damageNode.baseValue.GetConnections()[0]);
        }
    }
}
