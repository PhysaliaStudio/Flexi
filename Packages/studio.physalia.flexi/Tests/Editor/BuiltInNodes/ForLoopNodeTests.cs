using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class ForLoopNodeTests
    {
        [Test]
        public void IncrementLoop()
        {
            var forLoopNode = NodeFactory.Create<ForLoopNode>();
            var startIntNode = NodeFactory.Create<IntegerNode>();
            var endIntNode = NodeFactory.Create<IntegerNode>();
            var loopBodyNode = NodeFactory.Create<EmptyProcessNode>();
            var completedNode = NodeFactory.Create<EmptyProcessNode>();

            forLoopNode.startIndexPort.Connect(startIntNode.output);
            forLoopNode.endIndexPort.Connect(endIntNode.output);
            forLoopNode.loopBodyPort.Connect(loopBodyNode.previous);
            forLoopNode.completedPort.Connect(completedNode.previous);

            startIntNode.value.Value = 0;
            endIntNode.value.Value = 10;

            // In loop
            for (var i = 0; i < 10; i++)
            {
                forLoopNode.Run();
                Assert.AreEqual(loopBodyNode, forLoopNode.Next, "Expected to go to loop body but failed");
                Assert.AreEqual(i, forLoopNode.indexPort.GetValue());
            }

            // Out loop
            forLoopNode.Run();
            Assert.AreEqual(completedNode, forLoopNode.Next, "Expected to go to completed but failed");
            Assert.AreEqual(10, forLoopNode.indexPort.GetValue());
        }

        [Test]
        public void DecrementLoop()
        {
            var forLoopNode = NodeFactory.Create<ForLoopNode>();
            var startIntNode = NodeFactory.Create<IntegerNode>();
            var endIntNode = NodeFactory.Create<IntegerNode>();
            var loopBodyNode = NodeFactory.Create<EmptyProcessNode>();
            var completedNode = NodeFactory.Create<EmptyProcessNode>();

            forLoopNode.startIndexPort.Connect(startIntNode.output);
            forLoopNode.endIndexPort.Connect(endIntNode.output);
            forLoopNode.loopBodyPort.Connect(loopBodyNode.previous);
            forLoopNode.completedPort.Connect(completedNode.previous);

            startIntNode.value.Value = 10;
            endIntNode.value.Value = 0;

            // In loop
            for (var i = 9; i >= 0; i--)
            {
                forLoopNode.Run();
                Assert.AreEqual(loopBodyNode, forLoopNode.Next, "Expected to go to loop body but failed");
                Assert.AreEqual(i, forLoopNode.indexPort.GetValue());
            }

            // Out loop
            forLoopNode.Run();
            Assert.AreEqual(completedNode, forLoopNode.Next, "Expected to go to completed but failed");
            Assert.AreEqual(-1, forLoopNode.indexPort.GetValue());
        }

        [Test]
        public void StartIndexEqualsEndIndex_NoLoop()
        {
            var forLoopNode = NodeFactory.Create<ForLoopNode>();
            var startIntNode = NodeFactory.Create<IntegerNode>();
            var endIntNode = NodeFactory.Create<IntegerNode>();
            var loopBodyNode = NodeFactory.Create<EmptyProcessNode>();
            var completedNode = NodeFactory.Create<EmptyProcessNode>();

            forLoopNode.startIndexPort.Connect(startIntNode.output);
            forLoopNode.endIndexPort.Connect(endIntNode.output);
            forLoopNode.loopBodyPort.Connect(loopBodyNode.previous);
            forLoopNode.completedPort.Connect(completedNode.previous);

            startIntNode.value.Value = 5;
            endIntNode.value.Value = 5;

            // Out loop
            forLoopNode.Run();
            Assert.AreEqual(completedNode, forLoopNode.Next, "Expected to go to completed but failed");
            Assert.AreEqual(5, forLoopNode.indexPort.GetValue());  // The index does not increment because the loop never run
        }

        [Test]
        public void RunAfterLoopIsFinish_TheIndexDoesNotChange()
        {
            var forLoopNode = NodeFactory.Create<ForLoopNode>();
            var startIntNode = NodeFactory.Create<IntegerNode>();
            var endIntNode = NodeFactory.Create<IntegerNode>();
            var loopBodyNode = NodeFactory.Create<EmptyProcessNode>();
            var completedNode = NodeFactory.Create<EmptyProcessNode>();

            forLoopNode.startIndexPort.Connect(startIntNode.output);
            forLoopNode.endIndexPort.Connect(endIntNode.output);
            forLoopNode.loopBodyPort.Connect(loopBodyNode.previous);
            forLoopNode.completedPort.Connect(completedNode.previous);

            startIntNode.value.Value = 0;
            endIntNode.value.Value = 10;

            // In loop
            for (var i = 0; i < 10; i++)
            {
                forLoopNode.Run();
                Assert.AreEqual(loopBodyNode, forLoopNode.Next, "Expected to go to loop body but failed");
                Assert.AreEqual(i, forLoopNode.indexPort.GetValue());
            }

            // Out loop
            forLoopNode.Run();
            Assert.AreEqual(completedNode, forLoopNode.Next, "Expected to go to completed but failed");
            Assert.AreEqual(10, forLoopNode.indexPort.GetValue());

            // Run Again
            forLoopNode.Run();
            Assert.AreEqual(completedNode, forLoopNode.Next, "Expected to go to completed but failed");
            Assert.AreEqual(10, forLoopNode.indexPort.GetValue());
        }

        [Test]
        public void Reset_TheLoopRestarts()
        {
            var forLoopNode = NodeFactory.Create<ForLoopNode>();
            var startIntNode = NodeFactory.Create<IntegerNode>();
            var endIntNode = NodeFactory.Create<IntegerNode>();
            var loopBodyNode = NodeFactory.Create<EmptyProcessNode>();
            var completedNode = NodeFactory.Create<EmptyProcessNode>();

            forLoopNode.startIndexPort.Connect(startIntNode.output);
            forLoopNode.endIndexPort.Connect(endIntNode.output);
            forLoopNode.loopBodyPort.Connect(loopBodyNode.previous);
            forLoopNode.completedPort.Connect(completedNode.previous);

            startIntNode.value.Value = 0;
            endIntNode.value.Value = 10;

            // In loop
            for (var i = 0; i < 10; i++)
            {
                forLoopNode.Run();
                Assert.AreEqual(loopBodyNode, forLoopNode.Next, "Expected to go to loop body but failed");
                Assert.AreEqual(i, forLoopNode.indexPort.GetValue());
            }

            // Out loop
            forLoopNode.Run();
            Assert.AreEqual(completedNode, forLoopNode.Next, "Expected to go to completed but failed");
            Assert.AreEqual(10, forLoopNode.indexPort.GetValue());

            forLoopNode.Reset();

            // In loop
            for (var i = 0; i < 10; i++)
            {
                forLoopNode.Run();
                Assert.AreEqual(loopBodyNode, forLoopNode.Next, "Expected to go to loop body but failed");
                Assert.AreEqual(i, forLoopNode.indexPort.GetValue());
            }

            // Out loop
            forLoopNode.Run();
            Assert.AreEqual(completedNode, forLoopNode.Next, "Expected to go to completed but failed");
            Assert.AreEqual(10, forLoopNode.indexPort.GetValue());
        }
    }
}
