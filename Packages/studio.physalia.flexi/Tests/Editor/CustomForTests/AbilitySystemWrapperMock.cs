using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    public class AbilitySystemWrapperMock : IAbilitySystemWrapper
    {
        public void ResolveEvent(AbilitySystem abilitySystem, IEventContext eventContext)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<AbilityDataContainer> CollectStatRefreshContainers()
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<StatOwner> CollectStatRefreshOwners()
        {
            throw new System.NotImplementedException();
        }

        public void OnBeforeCollectModifiers()
        {
            throw new System.NotImplementedException();
        }

        public void ApplyModifiers(StatOwner statOwner)
        {
            throw new System.NotImplementedException();
        }
    }
}
