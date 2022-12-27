namespace Physalia.AbilityFramework
{
    public interface IAbilityFlow
    {
        public FlowNode Current { get; }

        public bool HasNext();
        public bool MoveNext();
        public void Reset();
    }
}
