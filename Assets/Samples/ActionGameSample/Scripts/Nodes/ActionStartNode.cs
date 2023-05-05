namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class ActionStartNode : EntryNode
    {
        public override bool CanExecute(IEventContext payload)
        {
            return true;
        }

        protected override AbilityState DoLogic()
        {
            (Actor as Unit).AbilitySlot.SetToDisabledState();
            return AbilityState.RUNNING;
        }
    }
}
