namespace Physalia.Flexi
{
    internal class AbilityPool : ObjectPool<Ability>
    {
        public AbilityPool(AbilityFactory factory, int startSize, PoolExpandMethod expandMethod = PoolExpandMethod.OneAtATime) :
            base(factory, startSize, expandMethod)
        {

        }
    }
}
