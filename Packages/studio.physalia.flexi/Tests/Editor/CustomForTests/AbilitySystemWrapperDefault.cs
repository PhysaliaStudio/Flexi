using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    public class AbilitySystemWrapperDefault : IAbilitySystemWrapper
    {
        private readonly DefaultModifierHandler modifierHandler = new();
        private readonly List<Actor> actors = new();

        public void AppendActor(Actor actor)
        {
            actors.Add(actor);
        }

        #region Implement IAbilitySystemWrapper
        public void ResolveEvent(AbilitySystem abilitySystem, IEventContext eventContext)
        {
            for (var i = 0; i < actors.Count; i++)
            {
                abilitySystem.TryEnqueueAbility(actors[i].AbilityContainers, eventContext);
            }
        }

        public IReadOnlyList<StatOwner> CollectStatRefreshOwners()
        {
            var result = new List<StatOwner>();
            result.AddRange(actors);
            return result;
        }

        public IReadOnlyList<AbilityDataContainer> CollectStatRefreshContainers()
        {
            var result = new List<AbilityDataContainer>();
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
