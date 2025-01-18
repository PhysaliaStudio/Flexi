namespace Physalia.Flexi
{
    public interface IStatsRefreshAlgorithm
    {
        void OnBeforeCollectModifiers(Actor actor);
        void ApplyModifiers(Actor actor);
    }
}
