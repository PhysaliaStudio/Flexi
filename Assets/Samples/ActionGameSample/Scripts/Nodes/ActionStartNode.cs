namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class ActionStartNode : DefaultEntryNode
    {
        protected override FlowState OnExecute()
        {
            Container.Unit.AbilitySlot.SetToDisabledState();
            return FlowState.Success;
        }
    }
}
