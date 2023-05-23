namespace Physalia.Flexi
{
    public interface IModifierAlgorithm
    {
        void RefreshStats(StatOwner owner);
        void RefreshStats(Actor actor);
    }
}
