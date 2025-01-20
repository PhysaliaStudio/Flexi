using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class Unit : Entity
    {
        private readonly IUnitData unitData;
        private readonly Dictionary<StatusData, int> statusTable = new();
        private readonly Dictionary<StatusData, DefaultAbilityContainer> statusToContainerTable = new();

        private int health;

        public IUnitData Data => unitData;

        public string Name => unitData.Name;
        public UnitType UnitType => unitData.UnitType;
        public int Health { get => health; set => health = value; }
        public int HealthMax => GetStat(StatId.HEALTH_MAX).CurrentValue;

        public Unit(IUnitData unitData)
        {
            this.unitData = unitData;
        }

        public override string ToString()
        {
            return Name;
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

        public void AddStatusStack(StatusData statusData, int stack)
        {
            if (statusTable.ContainsKey(statusData))
            {
                statusTable[statusData] += stack;
            }
            else
            {
                statusTable.Add(statusData, stack);
            }
        }

        public void RemoveStatusStack(StatusData statusData, int stack)
        {
            if (statusTable.ContainsKey(statusData))
            {
                statusTable[statusData] -= stack;
                if (statusTable[statusData] <= 0)
                {
                    statusTable.Remove(statusData);
                }
            }
        }

        public void AppendStatusContainer(StatusData statusData, DefaultAbilityContainer container)
        {
            if (statusToContainerTable.ContainsKey(statusData))
            {
                Debug.LogWarning($"[{nameof(Unit)}] AppendStatusContainer failed! StatusData already has a container (statusData = {statusData})");
                return;
            }

            statusToContainerTable.Add(statusData, container);
            AppendAbilityContainer(container);
        }

        public void RemoveStatusContainer(StatusData statusData)
        {
            bool success = statusToContainerTable.Remove(statusData, out DefaultAbilityContainer container);
            if (success)
            {
                RemoveAbilityContainer(container);
            }
        }
    }
}
