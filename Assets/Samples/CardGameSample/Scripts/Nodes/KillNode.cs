using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    [NodeCategory("Card Game Sample")]
    public class KillNode : DefaultProcessNode
    {
        public Inport<IReadOnlyList<Unit>> targetsPort;

        protected override FlowState OnExecute()
        {
            var targets = targetsPort.GetValue();
            if (targets.Count == 0)
            {
                return FlowState.Success;
            }

            for (var i = 0; i < targets.Count; i++)
            {
                int health = targets[i].Health;
                if (health <= 0)
                {
                    EnqueueEvent(new DeathEvent
                    {
                        target = targets[i],
                    });
                }
            }

            return FlowState.Success;
        }
    }
}
