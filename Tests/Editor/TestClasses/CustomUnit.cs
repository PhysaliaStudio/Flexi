namespace Physalia.AbilityFramework.Tests
{
    public class CustomUnit : Actor
    {
        private readonly CustomUnitData data;

        public string Name => data.name;

        public CustomUnit(CustomUnitData data, ICreateStatOwner ownerCreater) : base(ownerCreater)
        {
            this.data = data;
        }
    }
}
