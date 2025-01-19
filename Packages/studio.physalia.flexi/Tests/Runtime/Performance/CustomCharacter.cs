using System.Collections.Generic;

namespace Physalia.Flexi.PerformanceTests
{
    public class CustomCharacter : StatOwner
    {
        private readonly List<AbilityContainer> containers = new();

        public IReadOnlyList<AbilityContainer> AbilityContainers => containers;

        public void AppendAbilityContainer(AbilityContainer container)
        {
            containers.Add(container);
        }

        public void RemoveAbilityContainer(AbilityContainer container)
        {
            containers.Remove(container);
        }

        public void ClearAbilityContainers()
        {
            containers.Clear();
        }
    }
}
