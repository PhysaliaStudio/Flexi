namespace Physalia.Flexi.Tests
{
    public interface IStatsRefreshAlgorithm
    {
        void OnBeforeCollectModifiers(Actor actor);
        void ApplyModifiers(Actor actor);
    }
}
