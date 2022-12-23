using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

namespace Physalia.AbilityFramework.Tests
{
    public class GraphConverterTests
    {
        [NodeCategory("Built-in/[Test Custom]")]
        public class OwnerFilterNode : Node
        {
            public Outport<StatOwner> owners;
        }

        [Test]
        public void Serialize_Empty()
        {
            Graph graph = new Graph();

            var expected =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"variables\":[]," +
                "\"nodes\":[]," +
                "\"edges\":[]}";

            string json = JsonConvert.SerializeObject(graph);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void Serialize_Normal()
        {
            Graph graph = new Graph();

            StartNode startNode = graph.AddNewNode<StartNode>();
            CustomDamageNode damageNode = graph.AddNewNode<CustomDamageNode>();
            OwnerFilterNode filterNode = graph.AddNewNode<OwnerFilterNode>();
            IntegerNode intNode = graph.AddNewNode<IntegerNode>();
            LogNode logNode = graph.AddNewNode<LogNode>();

            startNode.next.Connect(damageNode.previous);
            damageNode.next.Connect(logNode.previous);
            damageNode.targets.Connect(filterNode.owners);
            damageNode.baseValue.Connect(intNode.output);

            // Intentionally change node id for easier test
            startNode.id = 1;
            damageNode.id = 2;
            filterNode.id = 3;
            intNode.id = 4;
            logNode.id = 5;

            var expected =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"variables\":[]," +
                "\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
                "{\"_id\":2,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.Tests.CustomDamageNode\"}," +
                "{\"_id\":3,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.Tests.GraphConverterTests+OwnerFilterNode\"}," +
                "{\"_id\":4,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.IntegerNode\",\"value\":0}," +
                "{\"_id\":5,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.LogNode\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
                "{\"id1\":2,\"port1\":\"next\",\"id2\":5,\"port2\":\"previous\"}," +
                "{\"id1\":3,\"port1\":\"owners\",\"id2\":2,\"port2\":\"targets\"}," +
                "{\"id1\":4,\"port1\":\"output\",\"id2\":2,\"port2\":\"baseValue\"}]}";

            string json = JsonConvert.SerializeObject(graph);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void Deserialize_Normal()
        {
            var json =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
                "{\"_id\":2,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.Tests.CustomDamageNode\"}," +
                "{\"_id\":3,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.Tests.GraphConverterTests+OwnerFilterNode\"}," +
                "{\"_id\":4,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.IntegerNode\",\"value\":0}," +
                "{\"_id\":5,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.LogNode\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
                "{\"id1\":2,\"port1\":\"next\",\"id2\":5,\"port2\":\"previous\"}," +
                "{\"id1\":3,\"port1\":\"owners\",\"id2\":2,\"port2\":\"targets\"}," +
                "{\"id1\":4,\"port1\":\"output\",\"id2\":2,\"port2\":\"baseValue\"}]}";

            Graph graph = JsonConvert.DeserializeObject<Graph>(json);

            Assert.AreEqual(5, graph.Nodes.Count);
            Assert.AreEqual(true, graph.Nodes[0] is StartNode);
            Assert.AreEqual(true, graph.Nodes[1] is CustomDamageNode);
            Assert.AreEqual(true, graph.Nodes[2] is OwnerFilterNode);
            Assert.AreEqual(true, graph.Nodes[3] is IntegerNode);
            Assert.AreEqual(true, graph.Nodes[4] is LogNode);

            var startNode = graph.Nodes[0] as StartNode;
            var damageNode = graph.Nodes[1] as CustomDamageNode;
            var filterNode = graph.Nodes[2] as OwnerFilterNode;
            var intNode = graph.Nodes[3] as IntegerNode;
            var logNode = graph.Nodes[4] as LogNode;

            Assert.AreEqual(1, graph.EntryNodes.Count);
            Assert.AreEqual(startNode, graph.EntryNodes[0]);

            Assert.AreEqual(damageNode, startNode.Next);
            Assert.AreEqual(startNode, damageNode.Previous);

            Assert.AreEqual(logNode, damageNode.Next);
            Assert.AreEqual(damageNode, logNode.Previous);

            Assert.AreEqual(filterNode.owners, damageNode.targets.GetConnections()[0]);
            Assert.AreEqual(intNode.output, damageNode.baseValue.GetConnections()[0]);
        }

        [Test]
        public void Deserialize_WithMissingPort()
        {
            var json =
                "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
                "{\"_id\":2,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":3,\"_type\":\"Physalia.AbilityFramework.StringNode\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
                "{\"id1\":3,\"port1\":\"value\",\"id2\":2,\"port2\":\"text\"}]}";

            Graph graph = JsonConvert.DeserializeObject<Graph>(json);

            Assert.AreEqual(3, graph.Nodes.Count);
            Assert.AreEqual(true, graph.Nodes[0] is StartNode);
            Assert.AreEqual(true, graph.Nodes[1] is AbilityFramework.LogNode);
            Assert.AreEqual(true, graph.Nodes[2] is StringNode);

            var startNode = graph.Nodes[0] as StartNode;
            var logNode = graph.Nodes[1] as AbilityFramework.LogNode;
            var stringNode = graph.Nodes[2] as StringNode;

            Assert.AreEqual(1, graph.EntryNodes.Count);
            Assert.AreEqual(startNode, graph.EntryNodes[0]);

            Assert.AreEqual(logNode, startNode.Next);
            Assert.AreEqual(startNode, logNode.Previous);

            var missingPort = stringNode.GetOutport("value") as MissingOutport;
            Assert.NotNull(missingPort);
            Assert.AreEqual(logNode.text, missingPort.GetConnections()[0]);
        }

        [Test]
        public void SerializeMacro_Empty()
        {
            Graph graph = new Graph();
            graph.AddSubgraphInOutNodes();

            var expected =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"$input\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[]}," +
                "\"$output\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[]}," +
                "\"variables\":[]," +
                "\"nodes\":[]," +
                "\"edges\":[]}";

            string json = JsonConvert.SerializeObject(graph);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void DeserializeMacro_Empty()
        {
            var json =
               "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"$input\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[]}," +
                "\"$output\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[]}," +
                "\"variables\":[]," +
                "\"nodes\":[]," +
                "\"edges\":[]}";

            Graph graph = JsonConvert.DeserializeObject<Graph>(json);

            Assert.NotNull(graph.GraphInputNode);
            Assert.NotNull(graph.GraphOutputNode);
            Assert.AreEqual(0, graph.Nodes.Count);
        }

        [Test]
        public void SerializeMacro_WithCustomPorts()
        {
            Graph graph = new Graph();
            graph.AddSubgraphInOutNodes();

            graph.GraphInputNode.position = new Vector2(1f, 2f);
            graph.GraphOutputNode.position = new Vector2(8f, 4f);

            graph.GraphInputNode.CreateOutport<int>("test1", true);
            graph.GraphOutputNode.CreateInport<string>("test2", true);

            var expected =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"$input\":{\"position\":{\"x\":1.0,\"y\":2.0},\"portDatas\":[{\"name\":\"test1\",\"type\":\"System.Int32\"}]}," +
                "\"$output\":{\"position\":{\"x\":8.0,\"y\":4.0},\"portDatas\":[{\"name\":\"test2\",\"type\":\"System.String\"}]}," +
                "\"variables\":[]," +
                "\"nodes\":[]," +
                "\"edges\":[]}";

            string json = JsonConvert.SerializeObject(graph);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void DeserializeMacro_WithCustomPorts()
        {
            var json =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"$input\":{\"position\":{\"x\":1.0,\"y\":2.0},\"portDatas\":[{\"name\":\"test1\",\"type\":\"System.Int32\"}]}," +
                "\"$output\":{\"position\":{\"x\":8.0,\"y\":4.0},\"portDatas\":[{\"name\":\"test2\",\"type\":\"System.String\"}]}," +
                "\"variables\":[]," +
                "\"nodes\":[]," +
                "\"edges\":[]}";

            Graph graph = JsonConvert.DeserializeObject<Graph>(json);

            Assert.NotNull(graph.GraphInputNode);
            Assert.NotNull(graph.GraphOutputNode);
            Assert.AreEqual(0, graph.Nodes.Count);

            Assert.AreEqual(new Vector2(1f, 2f), graph.GraphInputNode.position);
            Assert.AreEqual(new Vector2(8f, 4f), graph.GraphOutputNode.position);

            Outport outport = graph.GraphInputNode.GetOutport("test1");
            Assert.NotNull(outport, "The GraphInputNode doesn't receive custom ports");
            Assert.AreEqual(typeof(int), outport.ValueType);

            Inport inport = graph.GraphOutputNode.GetInport("test2");
            Assert.NotNull(inport, "The GraphOutputNode doesn't receive custom ports");
            Assert.AreEqual(typeof(string), inport.ValueType);
        }


        [Test]
        public void SerializeMacro_WithEdges()
        {
            Graph graph = new Graph();
            graph.AddSubgraphInOutNodes();

            graph.GraphInputNode.CreateOutport<int>("test1", true);
            graph.GraphOutputNode.CreateInport<int>("test2", true);

            graph.GraphInputNode.next.Connect(graph.GraphOutputNode.previous);
            graph.GraphInputNode.GetOutport("test1").Connect(graph.GraphOutputNode.GetInport("test2"));

            var expected =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"$input\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[{\"name\":\"test1\",\"type\":\"System.Int32\"}]}," +
                "\"$output\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[{\"name\":\"test2\",\"type\":\"System.Int32\"}]}," +
                "\"variables\":[]," +
                "\"nodes\":[]," +
                "\"edges\":[{\"id1\":-1,\"port1\":\"next\",\"id2\":-2,\"port2\":\"previous\"}," +
                "{\"id1\":-1,\"port1\":\"test1\",\"id2\":-2,\"port2\":\"test2\"}]}";

            string json = JsonConvert.SerializeObject(graph);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void DeserializeMacro_WithEdges()
        {
            var json =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"$input\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[{\"name\":\"test1\",\"type\":\"System.Int32\"}]}," +
                "\"$output\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[{\"name\":\"test2\",\"type\":\"System.Int32\"}]}," +
                "\"variables\":[]," +
                "\"nodes\":[]," +
                "\"edges\":[{\"id1\":-1,\"port1\":\"next\",\"id2\":-2,\"port2\":\"previous\"}," +
                "{\"id1\":-1,\"port1\":\"test1\",\"id2\":-2,\"port2\":\"test2\"}]}";

            Graph graph = JsonConvert.DeserializeObject<Graph>(json);
            Assert.AreEqual(graph.GraphOutputNode.previous, graph.GraphInputNode.next.GetConnections()[0], "'next' and 'previous' didn't connect");

            Outport test1 = graph.GraphInputNode.GetOutport("test1");
            Inport test2 = graph.GraphOutputNode.GetInport("test2");
            Assert.AreEqual(test2, test1.GetConnections()[0], "'test1' and 'test2' didn't connect");
        }

        [Test]
        public void Serialize_WithSubgraphNode()
        {
            Graph graph = new Graph();
            SubgraphNode subgraphNode = graph.AddNewNode<SubgraphNode>();
            subgraphNode.key = "1234";

            // Intentionally change node id for easier test
            subgraphNode.id = 1;

            var expected =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"variables\":[]," +
                "\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.SubgraphNode\",\"key\":\"1234\"}]," +
                "\"edges\":[]}";

            string json = JsonConvert.SerializeObject(graph);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void Deserialize_WithSubgraphNode()
        {
            var json =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"variables\":[]," +
                "\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.SubgraphNode\",\"key\":\"1234\"}]," +
                "\"edges\":[]}";

            Graph graph = JsonConvert.DeserializeObject<Graph>(json);

            var subgraphNode = graph.GetNode(1) as SubgraphNode;
            Assert.NotNull(subgraphNode);
            Assert.AreEqual("1234", subgraphNode.key);
        }

        [Test]
        public void Serialize_WithSubgraphNodeWithCustomPorts_TheEdgesShouldBeCorrect()
        {
            var macroJson =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"$input\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[{\"name\":\"test1\",\"type\":\"System.Int32\"}]}," +
                "\"$output\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[{\"name\":\"test2\",\"type\":\"System.Int32\"}]}," +
                "\"variables\":[]," +
                "\"nodes\":[]," +
                "\"edges\":[{\"id1\":-1,\"port1\":\"next\",\"id2\":-2,\"port2\":\"previous\"}," +
                "{\"id1\":-1,\"port1\":\"test1\",\"id2\":-2,\"port2\":\"test2\"}]}";
            var macroLibrary = new MacroLibrary { { "1234", macroJson } };

            Graph graph = new Graph();
            IntegerNode integerNode = graph.AddNewNode<IntegerNode>();
            SubgraphNode subgraphNode = macroLibrary.AddMacroNode(graph, "1234");

            Inport test1 = subgraphNode.GetInport("test1");
            integerNode.output.Connect(test1);

            // Intentionally change node id for easier test
            integerNode.id = 1;
            subgraphNode.id = 2;

            var expected =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"variables\":[]," +
                "\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.IntegerNode\",\"value\":0}," +
                "{\"_id\":2,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.SubgraphNode\",\"key\":\"1234\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"output\",\"id2\":2,\"port2\":\"test1\"}]}";

            string json = JsonConvert.SerializeObject(graph);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void Deserialize_WithSubgraphNodeWithCustomPorts_TheEdgesShouldBeCorrect()
        {
            var graphJson =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"variables\":[]," +
                "\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.IntegerNode\",\"value\":0}," +
                "{\"_id\":2,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.SubgraphNode\",\"key\":\"1234\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"output\",\"id2\":2,\"port2\":\"test1\"}]}";
            var macroJson =
                "{\"_type\":\"Physalia.AbilityFramework.Graph\"," +
                "\"$input\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[{\"name\":\"test1\",\"type\":\"System.Int32\"}]}," +
                "\"$output\":{\"position\":{\"x\":0.0,\"y\":0.0},\"portDatas\":[{\"name\":\"test2\",\"type\":\"System.Int32\"}]}," +
                "\"variables\":[]," +
                "\"nodes\":[]," +
                "\"edges\":[{\"id1\":-1,\"port1\":\"next\",\"id2\":-2,\"port2\":\"previous\"}," +
                "{\"id1\":-1,\"port1\":\"test1\",\"id2\":-2,\"port2\":\"test2\"}]}";

            var macroLibrary = new MacroLibrary { { "1234", macroJson } };
            Graph graph = JsonConvert.DeserializeObject<Graph>(graphJson);
            macroLibrary.SetUpMacroNodes(graph);

            var subgraphNode = graph.GetNode(2) as SubgraphNode;
            Inport test1 = subgraphNode.GetInport("test1");
            Outport test2 = subgraphNode.GetOutport("test2");
            Assert.NotNull(test1, "The port 'test1' was not created.");
            Assert.NotNull(test2, "The port 'test2' was not created.");

            var integerNode = graph.GetNode(1) as IntegerNode;
            var integerOutputConnections = integerNode.output.GetConnections();
            Assert.AreEqual(1, integerOutputConnections.Count, "The edge to 'test1' is not correct.");
            Assert.AreEqual(test1, integerOutputConnections[0], "The edge to 'test1' is not correct.");
        }
    }
}
