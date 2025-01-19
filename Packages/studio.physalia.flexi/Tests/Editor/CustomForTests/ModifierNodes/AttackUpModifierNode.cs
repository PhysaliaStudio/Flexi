namespace Physalia.Flexi.Tests
{
    public class AttackUpModifierNode : ProcessNode
    {
        protected override AbilityState DoLogic()
        {
            Container.Actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 10, StatModifier.Operator.ADD));
            return AbilityState.RUNNING;
        }
    }
}
