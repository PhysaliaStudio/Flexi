namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class SetSlotStatusNode : ProcessNode
    {
        public Variable<AbilitySlot.State> state;
        public Variable<int> cooldownMilliseconds;

        protected override AbilityState DoLogic()
        {
            AbilitySlot slot = (Actor as Unit).AbilitySlot;
            switch (state.Value)
            {
                case AbilitySlot.State.OPEN:
                    slot.SetToOpenState();
                    break;
                case AbilitySlot.State.RECAST:
                    slot.SetToRecastState();
                    break;
                case AbilitySlot.State.DISABLED:
                    slot.SetToDisabledState();
                    break;
                case AbilitySlot.State.COOLDOWN:
                    slot.SetToCooldownState(cooldownMilliseconds.Value);
                    break;
            }

            return AbilityState.RUNNING;
        }
    }
}
