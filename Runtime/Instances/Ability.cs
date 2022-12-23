using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public class Ability
    {
        private readonly AbilitySystem abilitySystem;
        private readonly AbilityData abilityData;

        private readonly List<BlackboardVariable> blackboard = new();
        private readonly List<AbilityFlow> abilityFlows = new();

        public AbilitySystem System => abilitySystem;
        public AbilityData Data => abilityData;

        internal IReadOnlyList<BlackboardVariable> Blackboard => blackboard;
        internal IReadOnlyList<AbilityFlow> Flows => abilityFlows;

        public Actor Actor { get; internal set; }

        internal Ability(AbilitySystem abilitySystem, AbilityData abilityData)
        {
            this.abilitySystem = abilitySystem;
            this.abilityData = abilityData;
        }

        internal void Initialize()
        {
            for (var i = 0; i < abilityData.blackboard.Count; i++)
            {
                BlackboardVariable blackboardVariable = abilityData.blackboard[i];
                BlackboardVariable clone = blackboardVariable.Clone();
                blackboard.Add(clone);
            }

            for (var i = 0; i < abilityData.graphJsons.Count; i++)
            {
                string graphJson = abilityData.graphJsons[i];
                AbilityFlow abilityFlow = abilitySystem.InstantiateAbilityFlow(this, i);
                abilityFlows.Add(abilityFlow);
            }
        }
    }
}
