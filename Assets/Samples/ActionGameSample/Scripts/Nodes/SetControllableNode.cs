namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class SetControllableNode : ProcessNode
    {
        public Variable<bool> controllable;

        protected override AbilityState DoLogic()
        {
            if (controllable)
            {
                Container.Actor.GetStat(StatId.CONTROLLABLE).CurrentBase = 1;
            }
            else
            {
                Container.Actor.GetStat(StatId.CONTROLLABLE).CurrentBase = 0;
            }
            return AbilityState.RUNNING;
        }
    }
}
