using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.AbilityFramework.Tests
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
        public void LoadAbilityGraph_WithMissingPort_LogError()
        {
            // Have 1 missing node and 1 missing port
            abilitySystem.LoadAbilityGraph(123456, CustomAbility.HELLO_WORLD_MISSING_ELEMENTS);

            // Log 1 error from NodeConverter + 2 error from AbilityGraphUtility
            StatTestHelper.LogAssert(LogType.Error);
            StatTestHelper.LogAssert(LogType.Error);
            StatTestHelper.LogAssert(LogType.Error);
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

            abilitySystem.EnqueueAbilityAndRun(instance, null);
            Assert.AreEqual(4, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);

            abilitySystem.EnqueueAbilityAndRun(instance, null);
            Assert.AreEqual(8, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void RunAbilityInstance_InstanceCanDoTheSameThingAsOriginal()
        {
            abilitySystem.LoadAbilityGraph(123456, CustomAbility.HELLO_WORLD);

            AbilityInstance instance = abilitySystem.GetAbilityInstance(123456);
            abilitySystem.EnqueueAbilityAndRun(instance, null);

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

            abilitySystem.EnqueueAbilityAndRun(instance, payload1);
            abilitySystem.EnqueueAbilityAndRun(instance, payload2);

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

            abilitySystem.EnqueueAbilityAndRun(instance1, payload1);
            abilitySystem.EnqueueAbilityAndRun(instance2, payload1);
            abilitySystem.EnqueueAbilityAndRun(instance2, payload2);

            Assert.AreEqual(2, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(23, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void TargetSelectionAbilitiy_ReceivesChoice()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.NORAML_ATTACK_SELECTION);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityInstance instance1 = abilitySystem.GetAbilityInstance(1);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.EnqueueAbilityAndRun(instance1, payload);

            Assert.IsNotNull(choiceContext);
            Assert.AreEqual(6, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);  // Damage should not occur
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveValidAnswer_EffectOccurred()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.NORAML_ATTACK_SELECTION);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityInstance instance1 = abilitySystem.GetAbilityInstance(1);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.EnqueueAbilityAndRun(instance1, payload);

            Assert.IsNotNull(choiceContext);

            var answerContext = new CustomSingleTargetAnswerContext { target = unit2 };
            abilitySystem.Resume(answerContext);

            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveInvalidAnswer_LogErrorAndEffectNotOccurred()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.NORAML_ATTACK_SELECTION);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityInstance instance1 = abilitySystem.GetAbilityInstance(1);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.EnqueueAbilityAndRun(instance1, payload);

            Assert.IsNotNull(choiceContext);

            var answerContext = new CustomSingleTargetAnswerContext { target = null };
            abilitySystem.Resume(answerContext);

            StatTestHelper.LogAssert(LogType.Error);
            Assert.AreEqual(6, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);  // Damage should not occur
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveCancellation()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.NORAML_ATTACK_SELECTION);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityInstance instance1 = abilitySystem.GetAbilityInstance(1);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.EnqueueAbilityAndRun(instance1, payload);

            Assert.IsNotNull(choiceContext);

            abilitySystem.Resume(new CancellationContext());

            // Nothing happened
            Assert.AreEqual(25, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachCondition_ModifierAppendedAndStatsAreCorrect()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            unit.Owner.SetStat(CustomStats.HEALTH, 3);

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

            abilitySystem.AppendAbility(unit, 1);
            abilitySystem.RefreshModifiers();

            Assert.AreEqual(1, unit.Owner.Modifiers.Count);
            Assert.AreEqual(3, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);

            unit.Owner.SetStat(CustomStats.HEALTH, 6);
            abilitySystem.RefreshModifiers();

            Assert.AreEqual(0, unit.Owner.Modifiers.Count);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(4, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachConditionWhileRunningSystem_ModifierNotAppendedAndStatsAreCorrect()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.NORAML_ATTACK);
            abilitySystem.LoadAbilityGraph(2, CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            abilitySystem.AppendAbility(unit2, 2);

            AbilityInstance instance = abilitySystem.GetAbilityInstance(1);
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

            abilitySystem.EnqueueAbilityAndRun(instance, payload1);
            abilitySystem.EnqueueAbilityAndRun(instance, payload2);

            Assert.AreEqual(1, unit2.Owner.Modifiers.Count);
            Assert.AreEqual(3, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(19, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ChainEffect_TriggerAnotherAbilityFromNodeByEvent_StatsAreCorrect()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.NORAML_ATTACK);
            abilitySystem.LoadAbilityGraph(2, CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            abilitySystem.AppendAbility(unit2, 2);

            AbilityInstance instance = abilitySystem.GetAbilityInstance(1);
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

            abilitySystem.EnqueueAbilityAndRun(instance, payload1);
            abilitySystem.EnqueueAbilityAndRun(instance, payload2);

            Assert.AreEqual(3, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(8, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(17, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ChainEffect_MultipleAbilities_TriggeredByCorrectOrder()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.NORMAL_ATTACK_5_TIMES);
            abilitySystem.LoadAbilityGraph(2, CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED);
            abilitySystem.LoadAbilityGraph(3, CustomAbility.COUNTER_ATTACK);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 64, attack = 1, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 10, attack = 1, });
            abilitySystem.AppendAbility(unit2, 2);
            abilitySystem.AppendAbility(unit2, 3);

            AbilityInstance instance = abilitySystem.GetAbilityInstance(1);
            var payload = new CustomNormalAttackPayload
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            abilitySystem.EnqueueAbilityAndRun(instance, payload);

            Assert.AreEqual(2, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(5, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(32, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiy_ForLoop_StatsAreCorrect()
        {
            abilitySystem.LoadAbilityGraph(1, CustomAbility.NORMAL_ATTACK_5_TIMES);

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityInstance instance = abilitySystem.GetAbilityInstance(1);
            var payload = new CustomNormalAttackPayload
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            abilitySystem.EnqueueAbilityAndRun(instance, payload);
            Assert.AreEqual(5, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }
    }
}
