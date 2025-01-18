namespace Physalia.Flexi
{
    public class AbilitySystemBuilder
    {
        private IAbilitySystemWrapper wrapper;
        private IStatsRefreshAlgorithm statsRefreshAlgorithm;
        private AbilityFlowRunner runner;

        public AbilitySystem Build()
        {
            if (wrapper == null)
            {
                throw new System.ArgumentException("IAbilitySystemWrapper is not set.");
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

            return new AbilitySystem(wrapper, runner);
        }

        public void SetWrapper(IAbilitySystemWrapper wrapper)
        {
            this.wrapper = wrapper;
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
