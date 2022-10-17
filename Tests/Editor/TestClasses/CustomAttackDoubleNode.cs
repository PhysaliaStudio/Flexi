using System.Collections.Generic;

namespace Physalia.AbilitySystem.Tests
{
    public class CustomAttackDoubleNode : ProcessNode
    {
        public Inport<List<StatOwner>> targetsPort;

        protected override AbilityState DoLogic()
        {
            List<StatOwner> targets = targetsPort.GetValue();
            for (var i = 0; i < targets.Count; i++)
            {
                Stat stat = targets[i].GetStat(CustomStats.ATTACK);
                stat.CurrentBase *= 2;
                targets[i].RefreshStats();
            }

            return AbilityState.RUNNING;
        }
    }
}
