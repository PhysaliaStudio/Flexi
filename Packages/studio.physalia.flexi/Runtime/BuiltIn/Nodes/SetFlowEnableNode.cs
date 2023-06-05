namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Common)]
    public class SetFlowEnableNode : ProcessNode
    {
        public Variable<int> index;
        public Variable<bool> enable;

        protected override AbilityState DoLogic()
        {
            int flowCount = Ability.Flows.Count;

            int index;
            if (this.index >= 0)
            {
                index = this.index;
            }
            else
            {
                index = flowCount + this.index;
            }

            if (index < 0 || index >= flowCount)
            {
                Logger.Error($"SetFlowEnable failed! IndexOutOfRange: {index}");
                return AbilityState.RUNNING;
            }

            Ability.Flows[index].SetEnable(enable);
            return AbilityState.RUNNING;
        }
    }
}
