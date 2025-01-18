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

        private void Awake()
        {
            Assert.IsNotNull(abilityAsset);

            var builder = new AbilitySystemBuilder();
            abilitySystem = builder.Build();

            AbilityDataSource abilityDataSource = abilityAsset.Data.CreateDataSource(0);
            for (var i = 0; i < abilityCountPerFrame; i++)
            {
                var character = new CustomCharacter(abilitySystem);
                var abilityDataContainer = new AbilityDataContainer { DataSource = abilityDataSource };
                character.AppendAbilityDataContainer(abilityDataContainer);

                characters.Add(character);
            }
        }

        private void Update()
        {
            for (var i = 0; i < characters.Count; i++)
            {
                abilitySystem.RefreshStatsAndModifiers();
            }

            for (var i = 0; i < characters.Count; i++)
            {
                abilitySystem.TryEnqueueAbility(characters[i].AbilityDataContainers, null);
            }

            abilitySystem.Run();
        }
    }
}
