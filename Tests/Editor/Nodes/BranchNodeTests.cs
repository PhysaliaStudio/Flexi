using NUnit.Framework;

namespace Physalia.AbilityFramework.Tests
{
    public class BranchNodeTests
    {
        [Test]
        public void ConditionIsTrue_NextGoesFromTruePort()
        {
            var branchNode = NodeFactory.Create<BranchNode>();
            var trueNode = NodeFactory.Create<TrueNode>();
            branchNode.conditionPort.Connect(trueNode.value);

            var ifTrueNode = NodeFactory.Create<RelayNode>();
            branchNode.truePort.Connect(ifTrueNode.previous);

            branchNode.Run();  // This triggers EvaluateInputs()
            Assert.AreEqual(ifTrueNode, branchNode.Next);
        }

        [Test]
        public void ConditionIsFalse_NextGoesFromFalsePort()
        {
            var branchNode = NodeFactory.Create<BranchNode>();
            var falseNode = NodeFactory.Create<FalseNode>();
            branchNode.conditionPort.Connect(falseNode.value);

            var ifFalseNode = NodeFactory.Create<RelayNode>();
            branchNode.falsePort.Connect(ifFalseNode.previous);

            branchNode.Run();  // This triggers EvaluateInputs()
            Assert.AreEqual(ifFalseNode, branchNode.Next);
        }

        [Test]
        public void ConditionIsTrue_NoConnectionFromTruePort_NextIsNull()
        {
            var branchNode = NodeFactory.Create<BranchNode>();
            var trueNode = NodeFactory.Create<TrueNode>();
            branchNode.conditionPort.Connect(trueNode.value);

            branchNode.Run();  // This triggers EvaluateInputs()
            Assert.AreEqual(null, branchNode.Next);
        }


        [Test]
        public void ConditionIsFalse_NoConnectionFromFalsePort_NextIsNull()
        {
            var branchNode = NodeFactory.Create<BranchNode>();
            var falseNode = NodeFactory.Create<FalseNode>();
            branchNode.conditionPort.Connect(falseNode.value);

            branchNode.Run();  // This triggers EvaluateInputs()
            Assert.AreEqual(null, branchNode.Next);
        }
    }
}
