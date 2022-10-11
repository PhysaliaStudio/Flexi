using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.AbilitySystem.Tests
{
    public class IntegrationTests
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
        public void CreateOwner_GetOwnerReturnsTheSameInstance()
        {
            StatOwner owner = abilitySystem.CreateOwner();
            Assert.AreEqual(owner, abilitySystem.GetOwner(owner.Id));
        }

        [Test]
        public void RemoveOwner_GetOwnerReturnsNull()
        {
            StatOwner owner = abilitySystem.CreateOwner();
            abilitySystem.RemoveOwner(owner);
            Assert.AreEqual(null, abilitySystem.GetOwner(owner.Id));
        }

        [Test]
        public void RunAbilityInstance_InstanceCanDoTheSameThingAsOriginal()
        {
            abilitySystem.LoadAbilityGraph(123456, CustomAbility.HELLO_WORLD);

            AbilityInstance instance = abilitySystem.GetAbilityInstance(123456);
            abilitySystem.AddToLast(instance, null);
            abilitySystem.Run();

            // Check if the instance can do the same thing
            LogAssert.Expect(LogType.Log, "Hello");
            LogAssert.Expect(LogType.Log, "World!");
        }

        [Test]
        public void ExecuteCustomNodesAndAbilitiy()
        {
            abilitySystem.LoadAbilityGraph(123456, CustomAbility.NORAML_ATTACK);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityInstance instance = abilitySystem.GetAbilityInstance(123456);
            var payload = new CustomNormalAttackPayload
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            abilitySystem.AddToLast(instance, payload);
            abilitySystem.Run();

            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(21, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }
    }
}
