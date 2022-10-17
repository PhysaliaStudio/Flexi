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
        public void AppendAbilityToOwner_OwnerOfInstanceReturnsAsExpected()
        {
            abilitySystem.LoadAbilityGraph(123456, CustomAbility.ATTACK_DOUBLE);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            AbilityInstance instance = abilitySystem.AppendAbility(unit, 123456);

            Assert.AreEqual(unit.Owner, instance.Owner);
        }

        [Test]
        public void ActivateInstance()
        {
            abilitySystem.LoadAbilityGraph(123456, CustomAbility.ATTACK_DOUBLE);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            AbilityInstance instance = abilitySystem.AppendAbility(unit, 123456);

            abilitySystem.ActivateInstance(instance, null);
            Assert.AreEqual(4, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);

            abilitySystem.ActivateInstance(instance, null);
            Assert.AreEqual(8, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void RunAbilityInstance_InstanceCanDoTheSameThingAsOriginal()
        {
            abilitySystem.LoadAbilityGraph(123456, CustomAbility.HELLO_WORLD);

            AbilityInstance instance = abilitySystem.GetAbilityInstance(123456);
            abilitySystem.ActivateInstance(instance, null);

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
            var payload1 = new CustomNormalAttackPayload
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            var payload2 = new CustomNormalAttackPayload
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            abilitySystem.ActivateInstance(instance, payload1);
            abilitySystem.ActivateInstance(instance, payload2);

            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(21, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiySequence()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.ATTACK_DECREASE);
            abilitySystem.LoadAbilityGraph(2, CustomAbility.NORAML_ATTACK);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityInstance instance1 = abilitySystem.GetAbilityInstance(1);
            AbilityInstance instance2 = abilitySystem.GetAbilityInstance(2);
            var payload1 = new CustomNormalAttackPayload
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            var payload2 = new CustomNormalAttackPayload
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            abilitySystem.ActivateInstance(instance1, payload1);
            abilitySystem.ActivateInstance(instance2, payload1);
            abilitySystem.ActivateInstance(instance2, payload2);

            Assert.AreEqual(2, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(23, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachCondition_ModifierAppendedAndStatsAreCorrect()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            unit.Owner.SetStat(CustomStats.HEALTH, 3);
            unit.Owner.RefreshStats();

            abilitySystem.AppendAbility(unit, 1);
            abilitySystem.RefreshModifiers();

            Assert.AreEqual(1, unit.Owner.Modifiers.Count);
            Assert.AreEqual(3, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_NotReachCondition_ModifierNotAppendedAndStatsAreCorrect()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            abilitySystem.AppendAbility(unit, 1);
            abilitySystem.RefreshModifiers();

            Assert.AreEqual(0, unit.Owner.Modifiers.Count);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(4, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachConditionThenMakeNotReach_ModifierRemovedAndStatsAreCorrect()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            unit.Owner.SetStat(CustomStats.HEALTH, 3);
            unit.Owner.RefreshStats();

            abilitySystem.AppendAbility(unit, 1);
            abilitySystem.RefreshModifiers();

            Assert.AreEqual(1, unit.Owner.Modifiers.Count);
            Assert.AreEqual(3, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);

            unit.Owner.SetStat(CustomStats.HEALTH, 6);
            unit.Owner.RefreshStats();
            abilitySystem.RefreshModifiers();

            Assert.AreEqual(0, unit.Owner.Modifiers.Count);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(4, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }
    }
}
