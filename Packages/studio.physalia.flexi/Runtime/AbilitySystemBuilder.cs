namespace Physalia.Flexi
{
    public class AbilitySystemBuilder
    {
        private IAbilitySystemWrapper wrapper;
        private AbilityFlowRunner runner;

        public AbilitySystem Build()
        {
            if (wrapper == null)
            {
                throw new System.ArgumentException("IAbilitySystemWrapper is not set.");
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

        public void SetRunner(AbilityFlowRunner runner)
        {
            this.runner = runner;
        }
    }
}
