using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Physalia.AbilityFramework.Tests
{
    public class NodeTests
    {
        [Test]
        public void AddDynamicInport()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            Inport inport = node.CreateInport<int>("dyna", true);

            Assert.AreEqual(true, inport.IsDynamic);
            Assert.AreEqual(1, node.DynamicInports.Count);
            Assert.AreEqual(inport, node.DynamicInports[0]);
        }

        [Test]
        public void AddDynamicOutport()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            Outport outport = node.CreateOutport<int>("dyna", true);

            Assert.AreEqual(true, outport.IsDynamic);
            Assert.AreEqual(1, node.DynamicOutports.Count);
            Assert.AreEqual(outport, node.DynamicOutports[0]);
        }

        [Test]
        public void RemoveDynamicInport()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            Inport inport = node.CreateInport<int>("dyna", true);
            node.RemoveInport(inport);

            Assert.AreEqual(0, node.DynamicInports.Count);
        }

        [Test]
        public void RemoveDynamicOutport()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            Outport outport = node.CreateOutport<int>("dyna", true);
            node.RemoveOutport(outport);

            Assert.AreEqual(0, node.DynamicOutports.Count);
        }

        [Test]
        public void TryRenamePort_PortWithOldNameDoesNotExist_LogsErrorAndReturnsFalse()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            bool success = node.TryRenamePort("abc", "def");

            TestUtilities.LogAssertAnyString(LogType.Error);
            Assert.AreEqual(false, success);
        }

        [Test]
        public void TryRenamePort_PortWithOldNameIsNotDynamic_LogsErrorAndReturnsFalse()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            _ = node.CreateInport<int>("abc");  // Create a static port
            bool success = node.TryRenamePort("abc", "def");

            TestUtilities.LogAssertAnyString(LogType.Error);
            Assert.AreEqual(false, success);
        }

        [Test]
        public void TryRenamePort_PortWithNewNameAlreadyExist_LogsErrorAndReturnsFalse()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            _ = node.CreateInport<int>("def");
            bool success = node.TryRenamePort("abc", "def");

            TestUtilities.LogAssertAnyString(LogType.Error);
            Assert.AreEqual(false, success);
        }

        [Test]
        public void TryRenamePort_Success_ThePortNameIsChanged()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            Inport inport = node.CreateInport<int>("abc", true);
            bool success = node.TryRenamePort("abc", "def");

            Assert.AreEqual(true, success);
            Assert.AreEqual("def", inport.Name);
        }

        [Test]
        public void TryRenamePort_Success_NoOtherPortCreated()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            Inport inport = node.CreateInport<int>("abc", true);

            _ = node.TryRenamePort("abc", "def");

            Assert.AreEqual(1, node.Ports.Count());
            Assert.AreEqual(null, node.GetPort("abc"));
            Assert.AreEqual(inport, node.GetPort("def"));
        }

        [Test]
        public void TryRenamePort_Success_TheConnectionsAreKept()
        {
            Node node = NodeFactory.Create<EmptyNode>();
            Node otherNode = NodeFactory.Create<EmptyNode>();
            Inport inport = node.CreateInport<int>("abc", true);
            Outport outport = otherNode.CreateOutport<int>("other");
            inport.Connect(outport);

            _ = node.TryRenamePort("abc", "def");

            IReadOnlyList<Port> connections = inport.GetConnections();
            Assert.AreEqual(1, connections.Count);
            Assert.AreEqual(outport, connections[0]);
        }
    }
}
