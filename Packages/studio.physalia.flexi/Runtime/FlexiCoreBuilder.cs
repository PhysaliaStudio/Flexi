namespace Physalia.Flexi
{
    public class FlexiCoreBuilder
    {
        private AbilityFlowRunner runner;
        private IFlexiEventResolver eventResolver;
        private IFlexiStatRefreshResolver statRefreshResolver;

        public FlexiCore Build()
        {
            runner ??= new LifoQueueRunner();
            eventResolver ??= new EmptyFlexiEventResolver();
            statRefreshResolver ??= new EmptyFlexiStatRefreshResolver();

            Logger.Info($"[{nameof(FlexiCoreBuilder)}] Runner Type: {runner.GetType().Name}");
            return new FlexiCore(runner, eventResolver, statRefreshResolver);
        }

        public void SetRunner(AbilityFlowRunner runner)
        {
            this.runner = runner;
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
