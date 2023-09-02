using System.Collections.Generic;
using Physalia.Flexi.PerformanceTests;
using UnityEngine;
using UnityEngine.Assertions;

namespace Physalia.Flexi.Tests
{
    public class PerformanceTestRoot : MonoBehaviour
    {
        [SerializeField]
        private AbilityAsset abilityAsset;
        [SerializeField]
        private int abilityCountPerFrame = 50;

        private AbilitySystem abilitySystem;
        private readonly List<CustomCharacter> characters = new();
        private readonly List<AbilityDataSource> abilityDataSources = new();

        private void Awake()
        {
            Assert.IsNotNull(abilityAsset);

            var builder = new AbilitySystemBuilder();
            abilitySystem = builder.Build();
            
            for (var i = 0; i < abilityCountPerFrame; i++)
            {
                characters.Add(new CustomCharacter(abilitySystem));

                AbilityDataSource abilityDataSource = abilityAsset.Data.CreateDataSource(0);
                abilityDataSources.Add(abilityDataSource);
            }
        }

        private void Update()
        {
            for (var i = 0; i < characters.Count; i++)
            {
                abilitySystem.RefreshStatsAndModifiers();
            }

            for (var i = 0; i < abilityDataSources.Count; i++)
            {
                abilitySystem.TryEnqueueAbility(null, abilityDataSources[i], null);
            }

            abilitySystem.Run();
        }
    }
}
