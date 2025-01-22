using System.Collections.Generic;

namespace Physalia.Flexi
{
    /// <summary>
    /// Ability is an instance of <see cref="AbilityData"/>, which is created by <see cref="FlexiCore"/>,
    /// and is a container of <see cref="AbilityFlow"/> and <see cref="BlackboardVariable"/>.
    /// </summary>
    public class Ability
    {
        private readonly FlexiCore flexiCore;
        private readonly AbilityHandle abilityHandle;

        private readonly List<BlackboardVariable> variableList = new();
        private readonly Dictionary<string, BlackboardVariable> variableTable = new();
        private readonly List<AbilityFlow> abilityFlows = new();

        private AbilityContainer container;

        public AbilityData Data => abilityHandle.Data;
        public AbilityHandle Handle => abilityHandle;

        public IReadOnlyList<BlackboardVariable> Blackboard => variableList;
        public IReadOnlyList<AbilityFlow> Flows => abilityFlows;
        internal AbilityContainer Container { get => container; set => container = value; }

        internal Ability(FlexiCore flexiCore, AbilityHandle abilityHandle)
        {
            this.flexiCore = flexiCore;
            this.abilityHandle = abilityHandle;
        }

        internal void Initialize()
        {
            AbilityData abilityData = abilityHandle.Data;
            for (var i = 0; i < abilityData.blackboard.Count; i++)
            {
                BlackboardVariable variable = abilityData.blackboard[i];
                if (string.IsNullOrWhiteSpace(variable.key))
                {
                    Logger.Warn($"[{nameof(Ability)}] {abilityHandle} has variable with empty key. This is invalid.");
                    continue;
                }

                if (variableTable.ContainsKey(variable.key))
                {
                    Logger.Warn($"[{nameof(Ability)}] {abilityHandle} has variable with duplicated key '{variable.key}'. Only the first will be applied.");
                    continue;
                }

                BlackboardVariable clone = variable.Clone();
                variableList.Add(clone);
                variableTable.Add(clone.key, clone);
            }

            AbilityGraphGroup group = abilityHandle.GraphGroup;
            for (var i = 0; i < group.graphs.Count; i++)
            {
                string json = group.graphs[i];
                AbilityGraph graph = AbilityGraphUtility.Deserialize("", json, flexiCore.MacroLibrary);
                var abilityFlow = new AbilityFlow(flexiCore, this, graph);
                abilityFlows.Add(abilityFlow);
            }
        }

        public bool HasVariable(string key)
        {
            return variableTable.ContainsKey(key);
        }

        public int GetVariable(string key)
        {
            bool success = variableTable.TryGetValue(key, out BlackboardVariable variable);
            if (success)
            {
                return variable.value;
            }

            Logger.Warn($"[{nameof(Ability)}] {abilityHandle} doesn't contain key '{key}', returns 0");
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
                Logger.Warn($"[{nameof(Ability)}] {abilityHandle} doesn't contain key '{key}'. Already added it instead.");
                BlackboardVariable variable = new BlackboardVariable { key = key, value = newValue };
                variableList.Add(variable);
                variableTable.Add(key, variable);
            }
        }

        /// <summary>
        /// Reset will be called when released. See <see cref="FlexiCore.ReleaseAbility"/>.
        /// </summary>
        internal void Reset()
        {
            for (var i = 0; i < abilityFlows.Count; i++)
            {
                abilityFlows[i].Reset();
            }
            Container = null;
        }
    }
}
