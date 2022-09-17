using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.AbilitySystem.Tests
{
    public class NodeConverterTests
    {
        [Serializable]
        public class TestNode : Node
        {
            public Inport<string> input;
            public Outport<int> output;
            public Variable<int> value;
        }

        [Test]
        public void SerializeCustomNode_Normal()
        {
            TestNode node = new TestNode
            {
                id = 1,
                position = new Vector2(200f, 100f),
                value = new Variable<int>(42),
            };
            var json = JsonConvert.SerializeObject(node);

            var expected = "{\"_id\":1,\"_position\":{\"x\":200.0,\"y\":100.0},\"_type\":\"Physalia.AbilitySystem.Tests.NodeConverterTests+TestNode\",\"value\":42}";
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void SerializeCustomNode_WithVariableNotDefined()
        {
            TestNode node = new TestNode
            {
                id = 1,
                position = new Vector2(200f, 100f),
                // The "value" field is null
            };
            var json = JsonConvert.SerializeObject(node);

            var expected = "{\"_id\":1,\"_position\":{\"x\":200.0,\"y\":100.0},\"_type\":\"Physalia.AbilitySystem.Tests.NodeConverterTests+TestNode\",\"value\":0}";
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void DeserializeCustomNode_Normal()
        {
            Node node = JsonConvert.DeserializeObject<Node>(
                "{\"_id\":1,\"_position\":{\"x\":200,\"y\":100},\"_type\":\"Physalia.AbilitySystem.Tests.NodeConverterTests+TestNode\",\"value\":42}");

            Assert.AreEqual(true, node is TestNode);
            Assert.AreEqual(1, node.id);
            Assert.AreEqual(new Vector2(200f, 100f), node.position);
            Assert.AreEqual(true, (node as TestNode).input != null);
            Assert.AreEqual(true, (node as TestNode).output != null);
            Assert.AreEqual(42, (node as TestNode).value.Value);
        }

        [Test]
        public void DeserializeCustomNode_MissingIdField()
        {
            Node node = JsonConvert.DeserializeObject<Node>(
                "{\"_position\":{\"x\":200,\"y\":100},\"_type\":\"Physalia.AbilitySystem.Tests.NodeConverterTests+TestNode\",\"value\":42}");

            Assert.AreEqual(true, node is TestNode);
            Assert.AreEqual(0, node.id);
            Assert.AreEqual(new Vector2(200f, 100f), node.position);
            Assert.AreEqual(42, (node as TestNode).value.Value);
        }

        [Test]
        public void DeserializeCustomNode_MissingPositionField()
        {
            Node node = JsonConvert.DeserializeObject<Node>(
                "{\"_id\":1,\"_type\":\"Physalia.AbilitySystem.Tests.NodeConverterTests+TestNode\",\"value\":42}");

            Assert.AreEqual(true, node is TestNode);
            Assert.AreEqual(1, node.id);
            Assert.AreEqual(new Vector2(0f, 0f), node.position);
            Assert.AreEqual(42, (node as TestNode).value.Value);
        }

        [Test]
        public void DeserializeCustomNode_MissingDataField()
        {
            Node node = JsonConvert.DeserializeObject<Node>(
                "{\"_id\":1,\"_position\":{\"x\":200,\"y\":100},\"_type\":\"Physalia.AbilitySystem.Tests.NodeConverterTests+TestNode\"}");

            Assert.AreEqual(true, node is TestNode);
            Assert.AreEqual(1, node.id);
            Assert.AreEqual(new Vector2(200f, 100f), node.position);
            Assert.AreEqual(0, (node as TestNode).value.Value);
        }

        [Test]
        public void DeserializeCustomNode_MissingTypeField()
        {
            Node node = JsonConvert.DeserializeObject<Node>(
                "{\"_id\":1,\"_position\":{\"x\":200,\"y\":100},\"value\":42}");

            LogAssert.Expect(LogType.Error, new Regex(".*"));
            Assert.AreEqual(true, node is UndefinedNode);
            Assert.AreEqual(1, node.id);
            Assert.AreEqual(new Vector2(200f, 100f), node.position);
        }

        [Test]
        public void DeserializeCustomNode_InvalidTypeField()
        {
            Node node = JsonConvert.DeserializeObject<Node>(
                "{\"_id\":1,\"_position\":{\"x\":200,\"y\":100},\"_type\":\"NonExistedNodeClass\",\"value\":42}");

            LogAssert.Expect(LogType.Error, new Regex(".*"));
            Assert.AreEqual(true, node is UndefinedNode);
            Assert.AreEqual(1, node.id);
            Assert.AreEqual(new Vector2(200f, 100f), node.position);
        }
    }
}
