using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public class Entity : StatOwner
    {
        private readonly List<AbilityContainer> containers = new();

        public IReadOnlyList<AbilityContainer> AbilityContainers => containers;

        public void AppendAbilityContainer(AbilityContainer container)
        {
            if (this is Unit unit)
            {
                container.unit = unit;
            }
            else if (this is Card card)
            {
                container.card = card;
            }

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
    }
}
