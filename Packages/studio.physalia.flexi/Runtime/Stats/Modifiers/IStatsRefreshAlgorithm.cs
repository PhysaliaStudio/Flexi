namespace Physalia.Flexi
{
    public interface IStatsRefreshAlgorithm
    {
        void OnBeforeCollectModifiers(Actor actor);
        void RefreshStats(Actor actor);
    }
}
