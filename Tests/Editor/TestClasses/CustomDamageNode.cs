using System.Collections.Generic;

namespace Physalia.AbilitySystem.Tests
{
    public class CustomDamageNode : ProcessNode
    {
        public Inport<CustomUnit> instigatorPort;
        public Inport<List<CustomUnit>> targets;
        public Inport<int> baseValue;

        protected override AbilityState DoLogic()
        {
            List<CustomUnit> list = targets.GetValue();
            int damage = baseValue.GetValue();
            for (var i = 0; i < list.Count; i++)
            {
                Stat stat = list[i].Owner.GetStat(CustomStats.HEALTH);
                stat.CurrentValue -= damage;
            }

            return AbilityState.RUNNING;
        }
    }
}
