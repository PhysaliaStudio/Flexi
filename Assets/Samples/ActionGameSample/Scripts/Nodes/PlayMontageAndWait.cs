namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class PlayMontageAndWait : ProcessNode
    {
        public Variable<string> name;
        public Variable<bool> disableControl;

        protected override AbilityState DoLogic()
        {
            IUnitAvatar avatar = (Actor as Unit).Avatar;
            if (!avatar.HasMontage(name.Value))
            {
                return AbilityState.RUNNING;
            }

            if (disableControl.Value)
            {
                Actor.GetStat(StatId.CONTROLLABLE).CurrentBase = 0;
            }

            avatar.PlayMontage(name.Value);
            return AbilityState.PAUSE;
        }

        protected override AbilityState Tick()
        {
            IUnitAvatar avatar = (Actor as Unit).Avatar;
            if (avatar.IsMontagePlayedAndFinished(name.Value))
            {
                if (disableControl.Value)
                {
                    Actor.GetStat(StatId.CONTROLLABLE).CurrentBase = 1;
                }

                return AbilityState.RUNNING;
            }

            return AbilityState.PAUSE;
        }
    }
}
