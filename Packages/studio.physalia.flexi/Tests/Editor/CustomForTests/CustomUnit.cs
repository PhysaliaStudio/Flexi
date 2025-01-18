namespace Physalia.Flexi.Tests
{
    public class CustomUnit : Actor
    {
        private readonly CustomUnitData data;

        public string Name => data.name;

        public CustomUnit(CustomUnitData data)
        {
            this.data = data;
        }
    }
}
