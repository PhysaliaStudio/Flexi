namespace Physalia.AbilityFramework.Tests
{
    public class CustomUnitFactory
    {
        private readonly AbilitySystem abilitySystem;

        public CustomUnitFactory(AbilitySystem abilitySystem)
        {
            this.abilitySystem = abilitySystem;
        }

        public CustomUnit Create(CustomUnitData data)
        {
            var unit = new CustomUnit(data, abilitySystem);
            unit.AddStat(CustomStats.HEALTH, data.health);
            unit.AddStat(CustomStats.MAX_HEALTH, data.health);
            unit.AddStat(CustomStats.ATTACK, data.attack);
            return unit;
        }
    }
}
