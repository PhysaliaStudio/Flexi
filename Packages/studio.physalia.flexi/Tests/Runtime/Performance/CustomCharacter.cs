using System.Collections.Generic;

namespace Physalia.Flexi.PerformanceTests
{
    public class CustomCharacter : StatOwner
    {
        private readonly List<DefaultAbilityContainer> containers = new();

        public IReadOnlyList<DefaultAbilityContainer> AbilityContainers => containers;

        public void AppendAbilityContainer(DefaultAbilityContainer container)
        {
            containers.Add(container);
        }

        public void RemoveAbilityContainer(DefaultAbilityContainer container)
        {
            containers.Remove(container);
        }

        public void ClearAbilityContainers()
        {
            containers.Clear();
        }
    }
}
