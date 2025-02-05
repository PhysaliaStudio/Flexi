namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class ActionStartNode : DefaultEntryNode<EmptyEventContext>
    {
        protected override bool CanExecute(EmptyEventContext context)
        {
            return true;
        }

        protected override FlowState OnExecute(EmptyEventContext context)
        {
            Container.Unit.AbilitySlot.SetToDisabledState();
            return FlowState.Success;
        }
    }
}
