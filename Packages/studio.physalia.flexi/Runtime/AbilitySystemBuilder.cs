using UnityEngine;

namespace Physalia.Flexi
{
    public class AbilitySystemBuilder
    {
        private StatDefinitionListAsset asset;
        private IStatsRefreshAlgorithm statsRefreshAlgorithm;
        private AbilityFlowRunner runner;

        public AbilitySystem Build()
        {
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
                Logger.Warn($"[{nameof(AbilitySystemBuilder)}] The stat definition asset is null. Internally created an empty one.");
            }

            if (statsRefreshAlgorithm == null)
            {
                statsRefreshAlgorithm = new DefaultStatsRefreshAlgorithm();
            }

            if (runner == null)
            {
                runner = new LifoQueueRunner();
            }

            Logger.Info($"[{nameof(AbilitySystemBuilder)}] Runner Type: {runner.GetType().Name}");

            return new AbilitySystem(asset, statsRefreshAlgorithm, runner);
        }

        public void SetStatDefinitions(StatDefinitionListAsset asset)
        {
            this.asset = asset;
        }

        public void SetModifierAlgorithm(IStatsRefreshAlgorithm statsRefreshAlgorithm)
        {
            this.statsRefreshAlgorithm = statsRefreshAlgorithm;
        }

        public void SetRunner(AbilityFlowRunner runner)
        {
            this.runner = runner;
        }
    }
}
