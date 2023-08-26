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
                Actor.GetStat(StatId.CONTROLLABLE).CurrentBase = 1;
            }
            else
            {
                Actor.GetStat(StatId.CONTROLLABLE).CurrentBase = 0;
            }
            return AbilityState.RUNNING;
        }
    }
}
