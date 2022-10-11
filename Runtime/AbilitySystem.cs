using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    public class AbilitySystem
    {
        private readonly StatOwnerRepository ownerRepository;
        private readonly AbilityRunner runner;

        private readonly Dictionary<int, string> graphTable = new();

        internal AbilitySystem(StatDefinitionListAsset statDefinitionListAsset, AbilityRunner runner)
        {
            ownerRepository = StatOwnerRepository.Create(statDefinitionListAsset);
            this.runner = runner;
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

        public void AddToLast(AbilityInstance instance, object payload)
        {
            instance.SetPayload(payload);
            runner.Add(instance);
        }

        public void Run()
        {
            runner.Run();
        }
    }
}
