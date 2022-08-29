using Physalia.AbilitySystem.StatSystem;

namespace Physalia.AbilitySystem
{
    public class AbilityContextInstance
    {
        private readonly AbilityContext data;

        public AbilityContextInstance(AbilityContext data)
        {
            this.data = data;
        }

        public void Calculate(StatOwner target)
        {
            for (var i = 0; i < data.Effects.Count; i++)
            {
                AbilityEffect execution = data.Effects[i];
                Stat stat = target.GetStat(execution.StatId);
                if (stat == null)
                {
                    continue;
                }

                int originalBase = stat.CurrentBase;
                switch (execution.Op)
                {
                    case AbilityEffect.Operator.SET:
                        target.SetStat(execution.StatId, execution.Value);
                        break;
                    case AbilityEffect.Operator.ADD:
                        target.SetStat(execution.StatId, originalBase + execution.Value);
                        break;
                    case AbilityEffect.Operator.MUL:
                        target.SetStat(execution.StatId, originalBase * (100 + execution.Value) / 100);
                        break;
                }
            }
        }
    }
}
