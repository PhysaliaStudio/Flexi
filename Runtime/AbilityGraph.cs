namespace Physalia.AbilitySystem
{
    public class AbilityGraph : Graph
    {
        public AbilityInstance CreateInstance()
        {
            var instance = new AbilityInstance(this);
            instance.Initialize();
            return instance;
        }
    }
}
