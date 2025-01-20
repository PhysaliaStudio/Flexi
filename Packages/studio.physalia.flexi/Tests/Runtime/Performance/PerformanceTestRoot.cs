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

            AbilityHandle abilityHandle = abilityAsset.Data.CreateHandle(0);
            for (var i = 0; i < abilityCountPerFrame; i++)
            {
                var character = new CustomCharacter();
                var abilityDataContainer = new DefaultAbilityContainer { Handle = abilityHandle };
                character.AppendAbilityContainer(abilityDataContainer);

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
                abilitySystem.TryEnqueueAbility(characters[i].AbilityContainers, null);
            }

            abilitySystem.Run();
        }
    }
}
