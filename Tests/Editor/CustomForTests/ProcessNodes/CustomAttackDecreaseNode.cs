using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomAttackDecreaseNode : ProcessNode
    {
        public Inport<List<CustomUnit>> targets;
        public Inport<int> baseValue;

        protected override AbilityState DoLogic()
        {
            List<CustomUnit> list = targets.GetValue();
            int damage = baseValue.GetValue();
            for (var i = 0; i < list.Count; i++)
            {
                Stat stat = list[i].Owner.GetStat(CustomStats.ATTACK);
                stat.CurrentBase -= damage;
            }

            return AbilityState.RUNNING;
        }
    }
}
