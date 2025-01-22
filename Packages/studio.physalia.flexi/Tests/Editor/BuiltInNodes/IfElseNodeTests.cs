using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class IfElseNodeTests
    {
        [Test]
        public void ConditionIsTrue_NextGoesFromTruePort()
        {
            var ifElseNode = NodeFactory.Create<IfElseNode>();
            var trueNode = NodeFactory.Create<TrueNode>();
            ifElseNode.conditionPort.Connect(trueNode.value);

            var ifTrueNode = NodeFactory.Create<EmptyProcessNode>();
            ifElseNode.truePort.Connect(ifTrueNode.previous);

            ifElseNode.Run();  // This triggers EvaluateInputs()
            Assert.AreEqual(ifTrueNode, ifElseNode.Next);
        }

        [Test]
        public void ConditionIsFalse_NextGoesFromFalsePort()
        {
            var ifElseNode = NodeFactory.Create<IfElseNode>();
            var falseNode = NodeFactory.Create<FalseNode>();
            ifElseNode.conditionPort.Connect(falseNode.value);

            var ifFalseNode = NodeFactory.Create<EmptyProcessNode>();
            ifElseNode.falsePort.Connect(ifFalseNode.previous);

            ifElseNode.Run();  // This triggers EvaluateInputs()
            Assert.AreEqual(ifFalseNode, ifElseNode.Next);
        }

        [Test]
        public void ConditionIsTrue_NoConnectionFromTruePort_NextIsNull()
        {
            var ifElseNode = NodeFactory.Create<IfElseNode>();
            var trueNode = NodeFactory.Create<TrueNode>();
            ifElseNode.conditionPort.Connect(trueNode.value);

            ifElseNode.Run();  // This triggers EvaluateInputs()
            Assert.AreEqual(null, ifElseNode.Next);
        }


        [Test]
        public void ConditionIsFalse_NoConnectionFromFalsePort_NextIsNull()
        {
            var ifElseNode = NodeFactory.Create<IfElseNode>();
            var falseNode = NodeFactory.Create<FalseNode>();
            ifElseNode.conditionPort.Connect(falseNode.value);

            ifElseNode.Run();  // This triggers EvaluateInputs()
            Assert.AreEqual(null, ifElseNode.Next);
        }
    }
}
