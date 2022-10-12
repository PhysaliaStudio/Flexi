namespace Physalia.AbilitySystem.Tests
{
    public class CustomUnit : IHasStatOwner
    {
        private readonly CustomUnitData data;
        private readonly StatOwner statOwner;

        public string Name => data.name;
        public StatOwner Owner => statOwner;

        public CustomUnit(CustomUnitData data, StatOwner statOwner)
        {
            this.data = data;
            this.statOwner = statOwner;
        }
    }
}
