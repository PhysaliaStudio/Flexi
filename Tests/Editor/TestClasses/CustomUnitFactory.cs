namespace Physalia.AbilitySystem.Tests
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
            StatOwner owner = abilitySystem.CreateOwner();
            owner.AddStat(CustomStats.HEALTH, data.health);
            owner.AddStat(CustomStats.MAX_HEALTH, data.health);
            owner.AddStat(CustomStats.ATTACK, data.attack);

            var unit = new CustomUnit(data, owner);
            return unit;
        }
    }
}
