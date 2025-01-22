namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class PlayMontageAndWait : DefaultProcessNode
    {
        public Variable<string> name;
        public Variable<bool> disableControl;

        protected override FlowState OnExecute()
        {
            IUnitAvatar avatar = Container.Unit.Avatar;
            if (!avatar.HasMontage(name.Value))
            {
                return FlowState.Success;
            }

            if (disableControl.Value)
            {
                Container.Unit.GetStat(StatId.CONTROLLABLE).CurrentBase = 0;
            }

            avatar.PlayMontage(name.Value);
            return FlowState.Pause;
        }

        protected override FlowState Tick()
        {
            IUnitAvatar avatar = Container.Unit.Avatar;
            if (avatar.IsMontagePlayedAndFinished(name.Value))
            {
                if (disableControl.Value)
                {
                    Container.Unit.GetStat(StatId.CONTROLLABLE).CurrentBase = 1;
                }

                return FlowState.Success;
            }

            return FlowState.Pause;
        }
    }
}
