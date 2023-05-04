using System.Collections.Generic;

namespace Physalia.Flexi
{
    /// <summary>
    /// Ability is an instance of <see cref="AbilityData"/>, which is created by <see cref="AbilitySystem"/>,
    /// and is a container of <see cref="AbilityFlow"/> and <see cref="BlackboardVariable"/>.
    /// </summary>
    public class Ability
    {
        private readonly AbilitySystem abilitySystem;
        private readonly AbilityData abilityData;

        private readonly List<BlackboardVariable> variableList = new();
        private readonly Dictionary<string, BlackboardVariable> variableTable = new();
        private readonly List<AbilityFlow> abilityFlows = new();

        public AbilitySystem System => abilitySystem;
        public AbilityData Data => abilityData;

        internal IReadOnlyList<BlackboardVariable> Blackboard => variableList;
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
                BlackboardVariable variable = abilityData.blackboard[i];
                if (string.IsNullOrWhiteSpace(variable.key))
                {
                    Logger.Warn($"[{nameof(Ability)}] '{Data.name}' has variable with empty key. This is invalid.");
                    continue;
                }

                if (variableTable.ContainsKey(variable.key))
                {
                    Logger.Warn($"[{nameof(Ability)}] '{Data.name}' has variable with duplicated key '{variable.key}'. Only the first will be applied.");
                    continue;
                }

                BlackboardVariable clone = variable.Clone();
                variableList.Add(clone);
                variableTable.Add(clone.key, clone);
            }

            for (var i = 0; i < abilityData.graphJsons.Count; i++)
            {
                AbilityFlow abilityFlow = abilitySystem.InstantiateAbilityFlow(this, i);
                abilityFlows.Add(abilityFlow);
            }
        }

        public int GetVariable(string key)
        {
            bool success = variableTable.TryGetValue(key, out BlackboardVariable variable);
            if (success)
            {
                return variable.value;
            }

            Logger.Warn($"[{nameof(Ability)}] '{Data.name}' doesn't contain key '{key}', returns 0");
            return 0;
        }

        public void OverrideVariable(string key, int newValue)
        {
            if (variableTable.ContainsKey(key))
            {
                variableTable[key].value = newValue;
            }
            else
            {
                Logger.Warn($"[{nameof(Ability)}] '{Data.name}' doesn't contain key '{key}'. Already added it instead.");
                BlackboardVariable variable = new BlackboardVariable { key = key, value = newValue };
                variableList.Add(variable);
                variableTable.Add(key, variable);
            }
        }
    }
}
