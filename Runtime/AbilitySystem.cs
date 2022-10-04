using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    public class AbilitySystem
    {
        private readonly StatOwnerRepository ownerRepository;
        private readonly Dictionary<int, string> graphTable = new();

        public AbilitySystem(StatDefinitionListAsset statDefinitionListAsset)
        {
            ownerRepository = StatOwnerRepository.Create(statDefinitionListAsset);
        }

        public StatOwner CreateOwner()
        {
            return ownerRepository.CreateOwner();
        }

        public void RemoveOwner(StatOwner owner)
        {
            ownerRepository.RemoveOwner(owner);
        }

        public StatOwner GetOwner(int id)
        {
            return ownerRepository.GetOwner(id);
        }

        public void LoadAbilityGraph(int id, string graphJson)
        {
            bool success = graphTable.TryAdd(id, graphJson);
            if (!success)
            {
                Debug.LogError($"[{nameof(AbilitySystem)}] Load graph failed! Already exists graph with Id:{id}");
            }
        }

        public AbilityInstance GetAbilityInstance(int id)
        {
            bool success = graphTable.TryGetValue(id, out string graphJson);
            if (!success)
            {
                Debug.LogError($"[{nameof(AbilitySystem)}] Get instance failed! Not exists graph with Id:{id}");
                return null;
            }

            AbilityGraph graph = JsonConvert.DeserializeObject<AbilityGraph>(graphJson);
            AbilityInstance instance = new AbilityInstance(graph);
            return instance;
        }
    }
}
