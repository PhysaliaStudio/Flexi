using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal class EmptyFlexiCoreWrapper : IFlexiCoreWrapper
    {
        private readonly List<StatOwner> EmptyOwnerList = new();
        private readonly List<AbilityContainer> EmptyContainers = new();

        public void OnEventReceived(IEventContext eventContext)
        {

        }

        public void ResolveEvent(FlexiCore flexiCore, IEventContext eventContext)
        {

        }

        public IReadOnlyList<StatOwner> CollectStatRefreshOwners()
        {
            return EmptyOwnerList;
        }

        public IReadOnlyList<AbilityContainer> CollectStatRefreshContainers()
        {
            return EmptyContainers;
        }

        public void OnBeforeCollectModifiers()
        {

        }

        public void ApplyModifiers(StatOwner statOwner)
        {

        }
    }
}
