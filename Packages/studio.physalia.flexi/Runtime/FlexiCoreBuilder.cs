namespace Physalia.Flexi
{
    public class FlexiCoreBuilder
    {
        private AbilityFlowRunner runner;
        private IFlexiRunnerResolver runnerResolver;
        private IFlexiEventResolver eventResolver;
        private IFlexiStatRefreshResolver statRefreshResolver;

        public FlexiCore Build()
        {
            runner ??= new LifoQueueRunner();
            runnerResolver ??= new EmptyFlexiRunnerResolver();
            eventResolver ??= new EmptyFlexiEventResolver();
            statRefreshResolver ??= new EmptyFlexiStatRefreshResolver();

            Logger.Info($"[{nameof(FlexiCoreBuilder)}] FlowRunner: {runner.GetType().Name}");
            Logger.Info($"[{nameof(FlexiCoreBuilder)}] RunnerResolver: {runnerResolver.GetType().Name}");
            Logger.Info($"[{nameof(FlexiCoreBuilder)}] EventResolver: {eventResolver.GetType().Name}");
            Logger.Info($"[{nameof(FlexiCoreBuilder)}] StatRefreshResolver: {statRefreshResolver.GetType().Name}");

            return new FlexiCore(runner, runnerResolver, eventResolver, statRefreshResolver);
        }

        public void SetRunner(AbilityFlowRunner runner)
        {
            this.runner = runner;
        }

        public void SetRunnerResolver(IFlexiRunnerResolver runnerResolver)
        {
            this.runnerResolver = runnerResolver;
        }

        public void SetEventResolver(IFlexiEventResolver eventResolver)
        {
            this.eventResolver = eventResolver;
        }

        public void SetStatRefreshResolver(IFlexiStatRefreshResolver statRefreshResolver)
        {
            this.statRefreshResolver = statRefreshResolver;
        }
    }
}
