namespace Physalia.Flexi
{
    public class AbilitySystemBuilder
    {
        private IStatsRefreshAlgorithm statsRefreshAlgorithm;
        private AbilityFlowRunner runner;

        public AbilitySystem Build()
        {
            if (statsRefreshAlgorithm == null)
            {
                statsRefreshAlgorithm = new DefaultStatsRefreshAlgorithm();
            }

            if (runner == null)
            {
                runner = new LifoQueueRunner();
            }

            Logger.Info($"[{nameof(AbilitySystemBuilder)}] Runner Type: {runner.GetType().Name}");

            return new AbilitySystem(statsRefreshAlgorithm, runner);
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
