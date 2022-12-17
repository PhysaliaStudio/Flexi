using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.AbilityFramework.Tests
{
    public class ActorIntegrationTests
    {
        private AbilitySystem abilitySystem;

        [SetUp]
        public void SetUp()
        {
            AbilitySystemBuilder builder = new AbilitySystemBuilder();

            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            statDefinitionListAsset.stats.AddRange(CustomStats.List);
            builder.SetStatDefinitions(statDefinitionListAsset);

            abilitySystem = builder.Build();
        }

        [Test]
        public void AddAbilityStack_Add5StackFrom0_GainsAnAbilityWith5Stack()
        {
            CustomUnitFactory unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            abilitySystem.LoadAbilityGraph(1, CustomAbility.POISON);

            abilitySystem.AddAbilityStack(unit1, 1, 5);

            AbilityInstance instance = unit1.FindAbility(1);
            Assert.NotNull(instance);
            Assert.AreEqual(5, instance.Stack);
        }

        [Test]
        public void AddAbilityStack_Add5StackFrom2_TheAbilityBecomes7Stack()
        {
            CustomUnitFactory unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            abilitySystem.LoadAbilityGraph(1, CustomAbility.POISON);
            abilitySystem.AddAbilityStack(unit1, 1, 2);

            abilitySystem.AddAbilityStack(unit1, 1, 5);

            AbilityInstance instance = unit1.FindAbility(1);
            Assert.AreEqual(1, unit1.Abilities.Count);
            Assert.AreEqual(7, instance.Stack);
        }

        [Test]
        public void RemoveAbilityStack_Remove3StackFrom5_TheAbilityBecomes2Stack()
        {
            CustomUnitFactory unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            abilitySystem.LoadAbilityGraph(1, CustomAbility.POISON);
            abilitySystem.AddAbilityStack(unit1, 1, 5);

            abilitySystem.RemoveAbilityStack(unit1, 1, 3);

            AbilityInstance instance = unit1.FindAbility(1);
            Assert.AreEqual(2, instance.Stack);
        }

        [Test]
        public void RemoveAbilityStack_Remove5StackFrom5_TheAbilityIsRemoved()
        {
            CustomUnitFactory unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            abilitySystem.LoadAbilityGraph(1, CustomAbility.POISON);
            abilitySystem.AddAbilityStack(unit1, 1, 5);

            abilitySystem.RemoveAbilityStack(unit1, 1, 5);

            AbilityInstance instance = unit1.FindAbility(1);
            Assert.IsNull(instance);
        }

        [Test]
        public void RemoveAbilityStack_Remove3StackFrom0_TheAbilityDoesNotExist()
        {
            CustomUnitFactory unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            abilitySystem.LoadAbilityGraph(1, CustomAbility.POISON);

            abilitySystem.RemoveAbilityStack(unit1, 1, 3);

            AbilityInstance instance = unit1.FindAbility(1);
            Assert.IsNull(instance);
        }
    }
}
