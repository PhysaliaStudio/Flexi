using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    public class FlexiCoreWrapperMock : IFlexiCoreWrapper
    {
        public void OnEventReceived(IEventContext eventContext)
        {
            throw new System.NotImplementedException();
        }

        public void ResolveEvent(FlexiCore flexiCore, IEventContext eventContext)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<AbilityContainer> CollectStatRefreshContainers()
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
