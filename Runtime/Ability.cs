namespace Physalia.AbilitySystem
{
    public class Ability : Graph
    {
        public AbilityInstance CreateInstance()
        {
            var instance = new AbilityInstance(this);
            instance.Initialize();
            return instance;
        }
    }
}
