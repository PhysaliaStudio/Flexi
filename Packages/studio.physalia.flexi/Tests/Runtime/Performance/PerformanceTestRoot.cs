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

        private FlexiCore flexiCore;
        private readonly List<CustomCharacter> characters = new();

        private void Awake()
        {
            Assert.IsNotNull(abilityAsset);

            var builder = new FlexiCoreBuilder();
            flexiCore = builder.Build();

            AbilityData abilityData = abilityAsset.Data;
            for (var i = 0; i < abilityCountPerFrame; i++)
            {
                var character = new CustomCharacter();
                var abilityDataContainer = new DefaultAbilityContainer(abilityData, 0);
                character.AppendAbilityContainer(abilityDataContainer);

                characters.Add(character);
            }
        }

        private void Update()
        {
            for (var i = 0; i < characters.Count; i++)
            {
                flexiCore.RefreshStatsAndModifiers();
            }

            for (var i = 0; i < characters.Count; i++)
            {
                flexiCore.TryEnqueueAbility(characters[i].AbilityContainers, null);
            }

            flexiCore.Run();
        }
    }
}
