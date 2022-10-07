namespace Physalia.AbilitySystem.Tests
{
    public class CustomUnitNode : ValueNode
    {
        public Inport<CustomUnit> customUnitPort;

        public Outport<int> healthPort;
        public Outport<int> maxHealthPort;
        public Outport<int> attackPort;

        public override void Evaluate()
        {
            CustomUnit customUnit = customUnitPort.GetValue();
            if (customUnit == null)
            {
                healthPort.SetValue(0);
                maxHealthPort.SetValue(0);
                attackPort.SetValue(0);
                return;
            }

            healthPort.SetValue(customUnit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            maxHealthPort.SetValue(customUnit.Owner.GetStat(CustomStats.MAX_HEALTH).CurrentValue);
            attackPort.SetValue(customUnit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }
    }
}
