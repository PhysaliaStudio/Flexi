namespace Physalia.AbilitySystem
{
    public class Ability : Graph
    {
        public AbilityInstance CreateInstance()
        {
            return new AbilityInstance(this);
        }
    }
}
