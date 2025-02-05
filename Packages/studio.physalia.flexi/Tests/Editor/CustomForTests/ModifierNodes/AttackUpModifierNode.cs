namespace Physalia.Flexi.Tests
{
    public class AttackUpModifierNode : DefaultModifierNode
    {
        protected override FlowState OnExecute()
        {
            Container.Actor.AppendModifier(StatModifier.Create(CustomStats.ATTACK, 10, StatModifier.Operator.ADD));
            return FlowState.Success;
        }
    }
}
