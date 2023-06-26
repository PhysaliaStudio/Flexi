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
        private readonly AbilityDataSource abilityDataSource;

        private readonly List<BlackboardVariable> variableList = new();
        private readonly Dictionary<string, BlackboardVariable> variableTable = new();
        private readonly List<AbilityFlow> abilityFlows = new();

        private object userData;

        public AbilitySystem System => abilitySystem;
        public AbilityData Data => abilityDataSource.AbilityData;
        public AbilityDataSource DataSource => abilityDataSource;

        public IReadOnlyList<BlackboardVariable> Blackboard => variableList;
        internal IReadOnlyList<AbilityFlow> Flows => abilityFlows;

        public Actor Actor { get; internal set; }

        internal Ability(AbilitySystem abilitySystem, AbilityDataSource abilityDataSource, object userData)
        {
            this.abilitySystem = abilitySystem;
            this.abilityDataSource = abilityDataSource;
            this.userData = userData;
        }

        internal void Initialize()
        {
            AbilityData abilityData = abilityDataSource.AbilityData;
            for (var i = 0; i < abilityData.blackboard.Count; i++)
            {
                BlackboardVariable variable = abilityData.blackboard[i];
                if (string.IsNullOrWhiteSpace(variable.key))
                {
                    Logger.Warn($"[{nameof(Ability)}] {abilityDataSource} has variable with empty key. This is invalid.");
                    continue;
                }

                if (variableTable.ContainsKey(variable.key))
                {
                    Logger.Warn($"[{nameof(Ability)}] {abilityDataSource} has variable with duplicated key '{variable.key}'. Only the first will be applied.");
                    continue;
                }

                BlackboardVariable clone = variable.Clone();
                variableList.Add(clone);
                variableTable.Add(clone.key, clone);
            }

            AbilityGraphGroup group = abilityDataSource.GraphGroup;
            for (var i = 0; i < group.graphs.Count; i++)
            {
                string json = group.graphs[i];
                AbilityFlow abilityFlow = abilitySystem.InstantiateAbilityFlow(this, json);
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

            Logger.Warn($"[{nameof(Ability)}] {abilityDataSource} doesn't contain key '{key}', returns 0");
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
                Logger.Warn($"[{nameof(Ability)}] {abilityDataSource} doesn't contain key '{key}'. Already added it instead.");
                BlackboardVariable variable = new BlackboardVariable { key = key, value = newValue };
                variableList.Add(variable);
                variableTable.Add(key, variable);
            }
        }

        public void SetEnable(int index, bool enable)
        {
            if (index < 0 || index >= abilityFlows.Count)
            {
                Logger.Warn($"[{nameof(Ability)}] {abilityDataSource} doesn't have flow at index '{index}'.");
                return;
            }

            abilityFlows[index].SetEnable(enable);
        }

        public T GetUserData<T>()
        {
            if (userData == null)
            {
                Logger.Warn($"[{nameof(Ability)}] {abilityDataSource} doesn't have userData. Returns null.");
                return default;
            }

            if (userData is T genericData)
            {
                return genericData;
            }
            else
            {
                Logger.Warn($"[{nameof(Ability)}] UserData in {abilityDataSource} is not type of '{typeof(T)}'. Returns null.");
                return default;
            }
        }

        public void SetUserData<T>(T userData)
        {
            this.userData = userData;
        }

        internal void Reset()
        {
            userData = null;
            for (var i = 0; i < abilityFlows.Count; i++)
            {
                abilityFlows[i].Reset();
            }
            Actor = null;
        }
    }
}
