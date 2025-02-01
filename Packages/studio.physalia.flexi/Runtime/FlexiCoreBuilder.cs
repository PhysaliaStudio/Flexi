namespace Physalia.Flexi
{
    public class FlexiCoreBuilder
    {
        private IFlexiCoreWrapper wrapper;
        private AbilityFlowRunner runner;

        public FlexiCore Build()
        {
            if (wrapper == null)
            {
                wrapper = new EmptyFlexiCoreWrapper();
            }

            if (runner == null)
            {
                runner = new LifoQueueRunner();
            }

            Logger.Info($"[{nameof(FlexiCoreBuilder)}] Runner Type: {runner.GetType().Name}");

            return new FlexiCore(wrapper, runner);
        }

        public void SetWrapper(IFlexiCoreWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        public void SetRunner(AbilityFlowRunner runner)
        {
            this.runner = runner;
        }
    }
}
