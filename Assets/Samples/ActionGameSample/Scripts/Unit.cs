using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class Unit : StatOwner
    {
        private readonly IUnitAvatar avatar;
        private readonly AbilitySlot abilitySlot = new();
        private readonly List<DefaultAbilityContainer> containers = new();

        public IUnitAvatar Avatar => avatar;
        public AbilitySlot AbilitySlot => abilitySlot;
        public IReadOnlyList<DefaultAbilityContainer> AbilityContainers => containers;

        public Unit(IUnitAvatar avatar)
        {
            this.avatar = avatar;
        }

        public void AppendAbilityContainer(DefaultAbilityContainer container)
        {
            container.Unit = this;
            containers.Add(container);
        }

        public void RemoveAbilityContainer(DefaultAbilityContainer container)
        {
            container.Unit = null;
            containers.Remove(container);
        }

        public void ClearAbilityContainers()
        {
            for (var i = 0; i < containers.Count; i++)
            {
                containers[i].Unit = null;
            }
            containers.Clear();
        }

        public bool IsControllable()
        {
            return GetStat(StatId.CONTROLLABLE).CurrentValue == 1;
        }

        public void Move(float x, float y)
        {
            float speedDelta = GetStat(StatId.SPEED).CurrentValue * 0.01f * Time.deltaTime;
            Avatar.Move(x * speedDelta, y * speedDelta);
        }

        public void Tick()
        {
            abilitySlot.Tick();
        }
    }
}
