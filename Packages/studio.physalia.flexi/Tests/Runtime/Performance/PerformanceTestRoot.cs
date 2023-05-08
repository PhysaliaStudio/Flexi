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
        private readonly List<Ability> abilities = new();

        private void Awake()
        {
            Assert.IsNotNull(abilityAsset);

            var builder = new AbilitySystemBuilder();
            abilitySystem = builder.Build();

            for (var i = 0; i < abilityCountPerFrame; i++)
            {
                characters.Add(new CustomCharacter(abilitySystem));

                Ability ability = abilitySystem.InstantiateAbility(abilityAsset);
                abilities.Add(ability);
            }
        }

        private void Update()
        {
            for (var i = 0; i < characters.Count; i++)
            {
                abilitySystem.RefreshStatsAndModifiers();
            }

            for (var i = 0; i < abilities.Count; i++)
            {
                abilitySystem.TryEnqueueAbility(abilities[i], null);
            }

            abilitySystem.Run();
        }
    }
}
