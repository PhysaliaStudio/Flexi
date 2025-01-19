using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    public abstract class Actor : StatOwner
    {
        private readonly List<AbilityContainer> containers = new(2);

        public IReadOnlyList<AbilityContainer> AbilityContainers => containers;

        public void AppendAbilityContainer(AbilityContainer container)
        {
            container.actor = this;
            containers.Add(container);
        }

        public void RemoveAbilityContainer(AbilityContainer container)
        {
            container.actor = null;
            containers.Remove(container);
        }

        public void ClearAllAbilityContainers()
        {
            for (var i = 0; i < containers.Count; i++)
            {
                containers[i].actor = null;
            }
            containers.Clear();
        }
    }
}
