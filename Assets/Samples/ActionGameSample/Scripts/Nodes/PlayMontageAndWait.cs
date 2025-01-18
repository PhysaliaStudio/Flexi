namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class PlayMontageAndWait : ProcessNode
    {
        public Variable<string> name;
        public Variable<bool> disableControl;

        protected override AbilityState DoLogic()
        {
            IUnitAvatar avatar = (Container.Actor as Unit).Avatar;
            if (!avatar.HasMontage(name.Value))
            {
                return AbilityState.RUNNING;
            }

            if (disableControl.Value)
            {
                Container.Actor.GetStat(StatId.CONTROLLABLE).CurrentBase = 0;
            }

            avatar.PlayMontage(name.Value);
            return AbilityState.PAUSE;
        }

        protected override AbilityState Tick()
        {
            IUnitAvatar avatar = (Container.Actor as Unit).Avatar;
            if (avatar.IsMontagePlayedAndFinished(name.Value))
            {
                if (disableControl.Value)
                {
                    Container.Actor.GetStat(StatId.CONTROLLABLE).CurrentBase = 1;
                }

                return AbilityState.RUNNING;
            }

            return AbilityState.PAUSE;
        }
    }
}
