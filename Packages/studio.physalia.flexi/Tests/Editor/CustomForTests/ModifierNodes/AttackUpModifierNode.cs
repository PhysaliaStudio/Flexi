namespace Physalia.Flexi.Tests
{
    public class AttackUpModifierNode : DefaultProcessNode
    {
        protected override FlowState OnExecute()
        {
            Container.Actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 10, StatModifier.Operator.ADD));
            return FlowState.Success;
        }
    }
}
