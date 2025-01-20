using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomAttackDoubleNode : DefaultProcessNode
    {
        public Inport<List<Actor>> targetsPort;

        protected override AbilityState DoLogic()
        {
            List<Actor> targets = targetsPort.GetValue();
            for (var i = 0; i < targets.Count; i++)
            {
                Stat stat = targets[i].GetStat(CustomStats.ATTACK);
                stat.CurrentBase *= 2;
            }

            return AbilityState.RUNNING;
        }
    }
}
