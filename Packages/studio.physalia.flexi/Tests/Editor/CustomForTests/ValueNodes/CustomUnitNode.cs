namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class CustomUnitNode : ValueNode
    {
        public Inport<CustomUnit> customUnitPort;

        public Outport<int> healthPort;
        public Outport<int> maxHealthPort;
        public Outport<int> attackPort;

        protected override void EvaluateSelf()
        {
            CustomUnit customUnit = customUnitPort.GetValue();
            if (customUnit == null)
            {
                healthPort.SetValue(0);
                maxHealthPort.SetValue(0);
                attackPort.SetValue(0);
                return;
            }

            healthPort.SetValue(customUnit.GetStat(CustomStats.HEALTH).CurrentValue);
            maxHealthPort.SetValue(customUnit.GetStat(CustomStats.MAX_HEALTH).CurrentValue);
            attackPort.SetValue(customUnit.GetStat(CustomStats.ATTACK).CurrentValue);
        }
    }
}
