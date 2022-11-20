namespace Physalia.AbilityFramework.Tests
{
    public class CustomUnit : Actor
    {
        private readonly CustomUnitData data;

        public string Name => data.name;

        public CustomUnit(CustomUnitData data, StatOwner statOwner) : base(statOwner)
        {
            this.data = data;
        }
    }
}
