using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public class Unit : StatOwner
    {
        private readonly IUnitData unitData;
        private readonly Dictionary<StatusData, int> statusTable = new();
        private readonly Dictionary<AbilityDataSource, AbilityContainer> sourceToContainerTable = new();

        private readonly List<AbilityContainer> containers = new();

        private int health;

        public IUnitData Data => unitData;

        public string Name => unitData.Name;
        public UnitType UnitType => unitData.UnitType;
        public int Health { get => health; set => health = value; }
        public int HealthMax => GetStat(StatId.HEALTH_MAX).CurrentValue;

        public IReadOnlyList<AbilityContainer> AbilityContainers => containers;

        public Unit(IUnitData unitData)
        {
            this.unitData = unitData;
        }

        public override string ToString()
        {
            return Name;
        }

        public void AppendAbilityContainer(AbilityContainer container)
        {
            container.unit = this;
            containers.Add(container);
        }

        public void RemoveAbilityContainer(AbilityContainer container)
        {
            container.CleanUp();
            containers.Remove(container);
        }

        public void ClearAbilityContainers()
        {
            for (var i = 0; i < containers.Count; i++)
            {
                containers[i].CleanUp();
            }
            containers.Clear();
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
                Logger.Warn($"[{nameof(Unit)}] AddAbilityStack failed! Given stack is less or equal to 0 (stack = {stack})");
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
                    var container = new AbilityContainer { DataSource = abilityDataSource };
                    sourceToContainerTable.Add(abilityDataSource, container);
                    AppendAbilityContainer(container);
                }
            }
        }

        public void RemoveAbilityStack(StatusData statusData, int stack)
        {
            if (stack <= 0)
            {
                Logger.Warn($"[{nameof(Unit)}] RemoveAbilityStack failed! Given stack is less or equal to 0 (stack = {stack})");
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
                        bool success = sourceToContainerTable.Remove(abilityDataSource, out AbilityContainer container);
                        if (success)
                        {
                            RemoveAbilityContainer(container);
                        }
                    }
                }
            }
        }
    }
}
