using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    public class AttackUpModifierNode : ProcessNode
    {
        protected override AbilityState DoLogic()
        {
            Actor.AppendModifier(new StatModifier
            {
                items = new List<StatModifierItem> {
                    new StatModifierItem
                    {
                        statId = CustomStats.ATTACK,
                        op = StatModifierItem.Operator.ADD,
                        value = 10,
                    },
                },
            });
            return AbilityState.RUNNING;
        }
    }
}
