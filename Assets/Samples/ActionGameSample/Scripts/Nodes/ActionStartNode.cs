namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class ActionStartNode : EntryNode
    {
        protected override AbilityState DoLogic()
        {
            SelfUnit.AbilitySlot.SetToDisabledState();
            return AbilityState.RUNNING;
        }
    }
}
