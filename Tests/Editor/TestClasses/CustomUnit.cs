namespace Physalia.AbilityFramework.Tests
{
    public class CustomUnit : Actor
    {
        private readonly CustomUnitData data;

        public string Name => data.name;

        public CustomUnit(CustomUnitData data, AbilitySystem abilitySystem) : base(abilitySystem)
        {
            this.data = data;
        }
    }
}
