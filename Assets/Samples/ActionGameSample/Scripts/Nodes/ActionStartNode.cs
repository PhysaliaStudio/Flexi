namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class ActionStartNode : DefaultEntryNode
    {
        protected override AbilityState OnExecute()
        {
            Container.Unit.AbilitySlot.SetToDisabledState();
            return AbilityState.RUNNING;
        }
    }
}
