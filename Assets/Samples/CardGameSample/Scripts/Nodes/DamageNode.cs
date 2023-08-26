using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class DamageNode : ProcessNode
    {
        public Inport<Unit> attackerPort;
        public Inport<IReadOnlyList<Unit>> targetsPort;
        public Inport<int> valuePort;

        protected override AbilityState DoLogic()
        {
            var attacker = attackerPort.GetValue();
            var targets = targetsPort.GetValue();
            if (targets.Count == 0)
            {
                return AbilityState.RUNNING;
            }

            var value = valuePort.GetValue();
            for (var i = 0; i < targets.Count; i++)
            {
                targets[i].Health -= value;
            }

            EnqueueEvent(new DamageEvent
            {
                attacker = attacker,
                targets = new List<Unit>(targets),
                amount = value,
            });

            return AbilityState.RUNNING;
        }
    }
}
