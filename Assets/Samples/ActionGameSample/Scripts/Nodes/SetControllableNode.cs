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
                SelfUnit.GetStat(StatId.CONTROLLABLE).CurrentBase = 1;
            }
            else
            {
                SelfUnit.GetStat(StatId.CONTROLLABLE).CurrentBase = 0;
            }
            return AbilityState.RUNNING;
        }
    }
}
