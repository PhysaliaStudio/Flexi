namespace Physalia.Flexi.Samples.CardGame
{
    public class SystemProcessContext : IEventContext
    {

    }

    [NodeCategory("Card Game Sample")]
    public class SystemProcessEntryNode : DefaultEntryNode<SystemProcessContext>
    {
        protected override bool CanExecute(SystemProcessContext context)
        {
            return true;
        }

        protected override FlowState OnExecute(SystemProcessContext context)
        {
            return FlowState.Success;
        }
    }
}
