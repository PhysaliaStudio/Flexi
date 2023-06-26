using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public class Unit : Actor
    {
        private readonly IUnitData unitData;
        private readonly Dictionary<StatusData, int> statusTable = new();

        public IUnitData Data => unitData;

        public string Name => unitData.Name;
        public UnitType UnitType => unitData.UnitType;

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
                    _ = AppendAbility(abilityDataSource);
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
                    AbilityData abilityData = statusData.AbilityAsset.Data;
                    for (var i = 0; i < abilityData.graphGroups.Count; i++)
                    {
                        AbilityDataSource abilityDataSource = abilityData.CreateDataSource(i);
                        _ = RemoveAbility(abilityDataSource);
                    }
                }
            }
        }
    }
}
