using System.Collections.Generic;

namespace Physalia.AbilitySystem
{
    public class ConditionalModifierNode : ProcessNode
    {
        public Inport<IReadOnlyList<StatOwner>> ownersPort;
        public Inport<bool> enabledPort;
        public Variable<StatModifier> modifierVariable;

        private StatModifierInstance modifierInstance;

        protected override AbilityState DoLogic()
        {
            IReadOnlyList<StatOwner> owners = ownersPort.GetValue();
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
