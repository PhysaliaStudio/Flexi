namespace Physalia.AbilityFramework.Tests
{
    [NodeCategory("Built-in/[Test Custom]")]
    public class CustomUnitTriggerNode : EntryNode
    {
        public Outport<CustomUnit> unitPort;

        public override bool CanExecute(IEventContext contextBase)
        {
            if (contextBase is CustomUnitTriggerContext context)
            {
                if (context.unit == Actor)
                {
                    return true;
                }
            }

            return false;
        }

        protected override AbilityState DoLogic()
        {
            var context = GetPayload<CustomUnitTriggerContext>();
            unitPort.SetValue(context.unit);
            return AbilityState.RUNNING;
        }
    }
}
