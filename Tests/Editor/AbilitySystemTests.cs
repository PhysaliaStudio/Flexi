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
        public void CreateAbilityInstance_WithMissingPort_LogError()
        {
            // Have 1 missing node and 1 missing port
            _ = abilitySystem.CreateAbilityFlow(CustomAbility.HELLO_WORLD_MISSING_ELEMENTS);

            // Log 1 error from NodeConverter + 2 error from AbilityGraphUtility
            TestUtilities.LogAssertAnyString(LogType.Error);
            TestUtilities.LogAssertAnyString(LogType.Error);
            TestUtilities.LogAssertAnyString(LogType.Error);
        }

        [Test]
        public void AppendAbilityToOwner_OwnerOfInstanceReturnsAsExpected()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            AbilityFlow abilityFlow = unit.AppendAbilityFlow(CustomAbility.ATTACK_DOUBLE);

            Assert.AreEqual(unit, abilityFlow.Actor);
        }

        [Test]
        public void ActivateInstance()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            AbilityFlow abilityFlow = unit.AppendAbilityFlow(CustomAbility.ATTACK_DOUBLE);

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, null);
            Assert.AreEqual(4, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, null);
            Assert.AreEqual(8, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void RunAbilityInstance_InstanceCanDoTheSameThingAsOriginal()
        {
            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.HELLO_WORLD);
            abilitySystem.EnqueueAbilityAndRun(abilityFlow, null);

            // Check if the instance can do the same thing
            LogAssert.Expect(LogType.Log, "Hello");
            LogAssert.Expect(LogType.Log, "World!");
        }

        [Test]
        public void ExecuteCustomNodesAndAbilitiy()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.NORAML_ATTACK);
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

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload1);
            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload2);

            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(21, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiySequence()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityFlow abilityFlow1 = abilitySystem.CreateAbilityFlow(CustomAbility.ATTACK_DECREASE);
            AbilityFlow abilityFlow2 = abilitySystem.CreateAbilityFlow(CustomAbility.NORAML_ATTACK);
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

            abilitySystem.EnqueueAbilityAndRun(abilityFlow1, payload1);
            abilitySystem.EnqueueAbilityAndRun(abilityFlow2, payload1);
            abilitySystem.EnqueueAbilityAndRun(abilityFlow2, payload2);

            Assert.AreEqual(2, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(23, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ConditionalEntryNode_WithConditionSuccess_ExecuteAsExpected()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { name = "Mob1", });

            AbilityFlow abilityFlow = unit.AppendAbilityFlow(CustomAbility.LOG_WHEN_ATTACKED);
            var context = new CustomDamageEvent { target = unit, };
            abilitySystem.EnqueueAbilityAndRun(abilityFlow, context);

            LogAssert.Expect(LogType.Log, "I'm damaged!");
            LogAssert.Expect(LogType.Log, "I will revenge!");
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void TargetSelectionAbilitiy_ReceivesChoice()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.NORAML_ATTACK_SELECTION);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload);

            Assert.IsNotNull(choiceContext);
            Assert.AreEqual(6, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);  // Damage should not occur
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveValidAnswer_EffectOccurred()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.NORAML_ATTACK_SELECTION);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload);

            Assert.IsNotNull(choiceContext);

            var answerContext = new CustomSingleTargetAnswerContext { target = unit2 };
            abilitySystem.Resume(answerContext);

            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveInvalidAnswer_LogErrorAndEffectNotOccurred()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.NORAML_ATTACK_SELECTION);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload);

            Assert.IsNotNull(choiceContext);

            var answerContext = new CustomSingleTargetAnswerContext { target = null };
            abilitySystem.Resume(answerContext);

            TestUtilities.LogAssertAnyString(LogType.Error);
            Assert.AreEqual(6, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);  // Damage should not occur
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveCancellation()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.NORAML_ATTACK_SELECTION);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload);

            Assert.IsNotNull(choiceContext);

            abilitySystem.Resume(new CancellationContext());

            // Nothing happened
            Assert.AreEqual(25, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachCondition_ModifierAppendedAndStatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            unit.SetStat(CustomStats.HEALTH, 3);

            _ = unit.AppendAbilityFlow(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);
            abilitySystem.RefreshModifiers();

            Assert.AreEqual(1, unit.Owner.Modifiers.Count);
            Assert.AreEqual(3, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_NotReachCondition_ModifierNotAppendedAndStatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            _ = unit.AppendAbilityFlow(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);
            abilitySystem.RefreshModifiers();

            Assert.AreEqual(0, unit.Owner.Modifiers.Count);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(4, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachConditionThenMakeNotReach_ModifierRemovedAndStatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            unit.SetStat(CustomStats.HEALTH, 3);
            _ = unit.AppendAbilityFlow(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);
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
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            _ = unit2.AppendAbilityFlow(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.NORAML_ATTACK);
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

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload1);
            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload2);

            Assert.AreEqual(1, unit2.Owner.Modifiers.Count);
            Assert.AreEqual(3, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(19, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ChainEffect_TriggerAnotherAbilityFromNodeByEvent_StatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            _ = unit2.AppendAbilityFlow(CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED);

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.NORAML_ATTACK);
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

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload1);
            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload2);

            Assert.AreEqual(3, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(8, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(17, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ChainEffect_MultipleAbilities_TriggeredByCorrectOrder()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 64, attack = 1, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 10, attack = 1, });
            _ = unit2.AppendAbilityFlow(CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED);
            _ = unit2.AppendAbilityFlow(CustomAbility.COUNTER_ATTACK);

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.NORMAL_ATTACK_5_TIMES);
            var payload = new CustomNormalAttackPayload
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload);

            Assert.AreEqual(2, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(5, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(32, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiy_ForLoop_StatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.NORMAL_ATTACK_5_TIMES);
            var payload = new CustomNormalAttackPayload
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            abilitySystem.EnqueueAbilityAndRun(abilityFlow, payload);
            Assert.AreEqual(5, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiy_Macro()
        {
            var macro = CustomAbility.HELLO_WORLD_MACRO;
            abilitySystem.LoadMacroGraph(macro.name, macro);

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.HELLO_WORLD_MACRO_CALLER);
            abilitySystem.EnqueueAbilityAndRun(abilityFlow, null);

            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "end");
        }

        [Test]
        public void ExecuteAbilitiy_LoopMacro5Times()
        {
            var macro = CustomAbility.HELLO_WORLD_MACRO;
            abilitySystem.LoadMacroGraph(macro.name, macro);

            AbilityFlow abilityFlow = abilitySystem.CreateAbilityFlow(CustomAbility.HELLO_WORLD_MACRO_CALLER_5_TIMES);
            abilitySystem.EnqueueAbilityAndRun(abilityFlow, null);

            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "end");
        }
    }
}
