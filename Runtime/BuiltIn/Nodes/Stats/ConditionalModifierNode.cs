using System.Collections.Generic;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Stats)]
    public class ConditionalModifierNode : ProcessNode
    {
        public Inport<IReadOnlyList<Actor>> actorsPort;
        public Inport<bool> enabledPort;
        public Variable<StatModifier> modifierVariable;

        private StatModifierInstance modifierInstance;

        protected override AbilityState DoLogic()
        {
            IReadOnlyList<Actor> owners = actorsPort.GetValue();
            bool enabled = enabledPort.GetValue();

            StatModifier modifier = modifierVariable.Value;
            if (modifierInstance == null)
            {
                modifierInstance = new StatModifierInstance(modifier);
            }

            for (var i = 0; i < owners.Count; i++)
            {
                if (enabled)
                {
                    owners[i].AppendModifier(modifierInstance);
                }
                else
                {
                    owners[i].RemoveModifier(modifierInstance);
                }
            }

            return AbilityState.RUNNING;
        }
    }
}
