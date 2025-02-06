using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class DamageNode : DefaultProcessNode
    {
        public Inport<Unit> attackerPort;
        public Inport<IReadOnlyList<Unit>> targetsPort;
        public Inport<int> valuePort;

        protected override FlowState OnExecute()
        {
            var targets = targetsPort.GetValue();
            if (targets.Count == 0)
            {
                return FlowState.Success;
            }

            Container.Game.Damage(attackerPort, targets, valuePort);
            return FlowState.Success;
        }
    }
}
