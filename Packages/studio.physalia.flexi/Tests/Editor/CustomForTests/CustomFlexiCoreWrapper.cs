using System;
using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    public class CustomFlexiCoreWrapper : IFlexiEventResolver, IFlexiStatRefreshResolver
    {
        public event Action ChoiceTriggered;

        private readonly DefaultModifierHandler modifierHandler = new();
        private readonly List<Actor> actors = new();

        public void AppendActor(Actor actor)
        {
            actors.Add(actor);
        }

        public void TriggerChoice()
        {
            ChoiceTriggered?.Invoke();
        }

        #region Implement IFlexiEventResolver
        public void OnEventReceived(IEventContext eventContext)
        {

        }

        public void ResolveEvent(FlexiCore flexiCore, IEventContext eventContext)
        {
            for (var i = 0; i < actors.Count; i++)
            {
                flexiCore.TryEnqueueAbility(actors[i].AbilityContainers, eventContext);
            }
        }
        #endregion

        #region Implement IFlexiStatRefreshResolver
        public IReadOnlyList<StatOwner> CollectStatRefreshOwners()
        {
            var result = new List<StatOwner>();
            result.AddRange(actors);
            return result;
        }

        public IReadOnlyList<AbilityContainer> CollectStatRefreshContainers()
        {
            var result = new List<AbilityContainer>();
            for (var i = 0; i < actors.Count; i++)
            {
                result.AddRange(actors[i].AbilityContainers);
            }
            return result;
        }

        public void OnBeforeCollectModifiers()
        {

        }

        public void ApplyModifiers(StatOwner statOwner)
        {
            modifierHandler.ApplyModifiers(statOwner);
        }
        #endregion
    }
}
