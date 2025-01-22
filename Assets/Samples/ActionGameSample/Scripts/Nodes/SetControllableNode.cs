namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class SetControllableNode : DefaultProcessNode
    {
        public Variable<bool> controllable;

        protected override FlowState OnExecute()
        {
            if (controllable)
            {
                Container.Unit.GetStat(StatId.CONTROLLABLE).CurrentBase = 1;
            }
            else
            {
                Container.Unit.GetStat(StatId.CONTROLLABLE).CurrentBase = 0;
            }
            return FlowState.Success;
        }
    }
}
