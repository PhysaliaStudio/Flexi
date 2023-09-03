using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public class Unit : Actor
    {
        private readonly IUnitData unitData;
        private readonly Dictionary<StatusData, int> statusTable = new();
        private readonly Dictionary<AbilityDataSource, AbilityDataContainer> sourceToContainerTable = new();

        private int health;

        public IUnitData Data => unitData;

        public string Name => unitData.Name;
        public UnitType UnitType => unitData.UnitType;
        public int Health { get => health; set => health = value; }
        public int HealthMax => GetStat(StatId.HEALTH_MAX).CurrentValue;

        public Unit(IUnitData unitData, AbilitySystem abilitySystem) : base(abilitySystem)
        {
            this.unitData = unitData;
        }

        public override string ToString()
        {
            return $"{OwnerId}-{Name}";
        }

        public int GetStatusStack(StatusData statusData)
        {
            bool success = statusTable.TryGetValue(statusData, out int stack);
            if (success)
            {
                return stack;
            }
            else
            {
                return 0;
            }
        }

        public void AddAbilityStack(StatusData statusData, int stack)
        {
            if (stack <= 0)
            {
                Logger.Warn($"[{nameof(Actor)}] AddAbilityStack failed! Given stack is less or equal to 0 (stack = {stack})");
                return;
            }

            if (statusTable.ContainsKey(statusData))
            {
                statusTable[statusData] += stack;
            }
            else
            {
                statusTable.Add(statusData, stack);

                AbilityData abilityData = statusData.AbilityAsset.Data;
                for (var i = 0; i < abilityData.graphGroups.Count; i++)
                {
                    AbilityDataSource abilityDataSource = abilityData.CreateDataSource(i);
                    var container = new AbilityDataContainer { DataSource = abilityDataSource };
                    sourceToContainerTable.Add(abilityDataSource, container);
                    AppendAbilityDataContainer(container);
                }
            }
        }

        public void RemoveAbilityStack(StatusData statusData, int stack)
        {
            if (stack <= 0)
            {
                Logger.Warn($"[{nameof(Actor)}] RemoveAbilityStack failed! Given stack is less or equal to 0 (stack = {stack})");
                return;
            }

            if (statusTable.ContainsKey(statusData))
            {
                statusTable[statusData] -= stack;
                if (statusTable[statusData] <= 0)
                {
                    statusTable.Remove(statusData);

                    AbilityData abilityData = statusData.AbilityAsset.Data;
                    for (var i = 0; i < abilityData.graphGroups.Count; i++)
                    {
                        AbilityDataSource abilityDataSource = abilityData.CreateDataSource(i);
                        bool success = sourceToContainerTable.Remove(abilityDataSource, out AbilityDataContainer container);
                        if (success)
                        {
                            _ = RemoveAbilityDataContainer(container);
                        }
                    }
                }
            }
        }
    }
}
