namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class PlayMontageAndWait : DefaultProcessNode
    {
        public Variable<string> name;
        public Variable<bool> disableControl;

        protected override AbilityState DoLogic()
        {
            IUnitAvatar avatar = Container.Unit.Avatar;
            if (!avatar.HasMontage(name.Value))
            {
                return AbilityState.RUNNING;
            }

            if (disableControl.Value)
            {
                Container.Unit.GetStat(StatId.CONTROLLABLE).CurrentBase = 0;
            }

            avatar.PlayMontage(name.Value);
            return AbilityState.PAUSE;
        }

        protected override AbilityState Tick()
        {
            IUnitAvatar avatar = Container.Unit.Avatar;
            if (avatar.IsMontagePlayedAndFinished(name.Value))
            {
                if (disableControl.Value)
                {
                    Container.Unit.GetStat(StatId.CONTROLLABLE).CurrentBase = 1;
                }

                return AbilityState.RUNNING;
            }

            return AbilityState.PAUSE;
        }
    }
}
