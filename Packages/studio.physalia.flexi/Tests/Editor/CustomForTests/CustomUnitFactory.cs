namespace Physalia.Flexi.Tests
{
    public class CustomUnitFactory
    {
        public CustomUnitFactory()
        {

        }

        public CustomUnit Create(CustomUnitData data)
        {
            var unit = new CustomUnit(data);
            unit.AddStat(CustomStats.HEALTH, data.health);
            unit.AddStat(CustomStats.MAX_HEALTH, data.health);
            unit.AddStat(CustomStats.ATTACK, data.attack);
            return unit;
        }
    }
}
