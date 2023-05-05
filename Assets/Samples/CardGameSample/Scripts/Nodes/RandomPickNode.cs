using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class RandomPickNode : ValueNode
    {
        public Inport<IReadOnlyList<Unit>> sourcePort;
        public Outport<Unit> resultPort;

        protected override void EvaluateSelf()
        {
            IReadOnlyList<Unit> source = sourcePort.GetValue();
            var payload = GetPayload<PlayCardNode.Payload>();
            Unit unit = source.RandomPickOne(payload.random);
            resultPort.SetValue(unit);
        }
    }
}
