namespace Physalia.Flexi.Samples.ActionGame
{
    [NodeCategory("Action Game Sample")]
    public class SetControllableNode : ProcessNode
    {
        public Variable<bool> controllable;

        protected override AbilityState DoLogic()
        {
            Actor.SetStat(StatId.CONTROLLABLE, controllable.Value ? 1 : 0);
            return AbilityState.RUNNING;
        }
    }
}
