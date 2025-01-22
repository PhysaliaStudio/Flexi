using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class RandomPickNode : DefaultValueNode
    {
        public Inport<IReadOnlyList<Unit>> sourcePort;
        public Outport<Unit> resultPort;

        protected override void EvaluateSelf()
        {
            IReadOnlyList<Unit> source = sourcePort.GetValue();
            Unit unit = source.RandomPickOne(Container.Random);
            resultPort.SetValue(unit);
        }
    }
}
