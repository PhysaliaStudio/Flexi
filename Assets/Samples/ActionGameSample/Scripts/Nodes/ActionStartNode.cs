namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class ActionStartNode : EntryNode<EmptyContext>
    {
        public override bool CanExecute(EmptyContext context)
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
