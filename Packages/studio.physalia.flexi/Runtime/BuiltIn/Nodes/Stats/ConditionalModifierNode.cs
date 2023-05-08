using System.Collections.Generic;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Stats)]
    public class ConditionalModifierNode : ProcessNode
    {
        public Inport<IReadOnlyList<Actor>> actorsPort;
        public Inport<bool> enabledPort;
        public Variable<StatModifier> modifierVariable;

        protected override AbilityState DoLogic()
        {
            IReadOnlyList<Actor> owners = actorsPort.GetValue();
            StatModifier modifier = modifierVariable.Value;
            if (enabledPort.GetValue())
            {
                for (var i = 0; i < owners.Count; i++)
                {
                    owners[i].AppendModifier(modifier);
                }
            }

            return AbilityState.RUNNING;
        }
    }
}
