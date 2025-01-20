using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public class Entity : StatOwner
    {
        private readonly List<DefaultAbilityContainer> containers = new();

        public IReadOnlyList<DefaultAbilityContainer> AbilityContainers => containers;

        public void AppendAbilityContainer(DefaultAbilityContainer container)
        {
            if (this is Unit unit)
            {
                container.Unit = unit;
            }
            else if (this is Card card)
            {
                container.Card = card;
            }

            containers.Add(container);
        }

        public void RemoveAbilityContainer(DefaultAbilityContainer container)
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
