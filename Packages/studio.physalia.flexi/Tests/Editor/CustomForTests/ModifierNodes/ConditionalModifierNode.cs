using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class ConditionalModifierNode : DefaultModifierNode
    {
        public Inport<IReadOnlyList<Actor>> actorsPort;
        public Inport<bool> enabledPort;
        public Variable<List<StatModifier>> modifiers;

        protected override FlowState OnExecute()
        {
            IReadOnlyList<Actor> owners = actorsPort.GetValue();
            if (enabledPort.GetValue())
            {
                for (var i = 0; i < owners.Count; i++)
                {
                    owners[i].AppendModifiers(modifiers.Value);
                }
            }

            return FlowState.Success;
        }
    }
}
