using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.Flexi.Tests
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
            var emptyActor = new EmptyActor(abilitySystem);
            Assert.AreEqual(emptyActor.Owner, abilitySystem.GetOwner(emptyActor.OwnerId));
            Assert.AreEqual(emptyActor, abilitySystem.GetActor(emptyActor.OwnerId));
            Assert.AreEqual(true, emptyActor.Owner.Id == emptyActor.OwnerId, "The Id from Owner and Actor is different.");
        }

        [Test]
        public void RemoveOwner_GetOwnerReturnsNull()
        {
            var emptyActor = new EmptyActor(abilitySystem);
            abilitySystem.DestroyOwner(emptyActor);
            Assert.AreEqual(null, abilitySystem.GetOwner(emptyActor.Owner.Id));
            Assert.AreEqual(null, abilitySystem.GetActor(emptyActor.OwnerId));
        }

        [Test]
        public void InstantiateAbility_WithMissingPort_LogError()
        {
            // Have 1 missing node and 1 missing port
            _ = abilitySystem.InstantiateAbility(CustomAbility.HELLO_WORLD_MISSING_ELEMENTS);

            // Log 1 error from NodeConverter + 2 error from AbilityGraphUtility
            TestUtilities.LogAssertAnyString(LogType.Error);
            TestUtilities.LogAssertAnyString(LogType.Error);
            TestUtilities.LogAssertAnyString(LogType.Error);
        }

        [Test]
        public void ActivateInstance()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            Ability ability = unit.AppendAbility(CustomAbility.ATTACK_DOUBLE);

            abilitySystem.TryEnqueueAndRunAbility(ability, null);
            Assert.AreEqual(4, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);

            abilitySystem.TryEnqueueAndRunAbility(ability, null);
            Assert.AreEqual(8, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void RunAbilityInstance_InstanceCanDoTheSameThingAsOriginal()
        {
            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.HELLO_WORLD);
            abilitySystem.TryEnqueueAndRunAbility(ability, null);

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

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.NORAML_ATTACK);
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

            abilitySystem.TryEnqueueAndRunAbility(ability, payload1);
            abilitySystem.TryEnqueueAndRunAbility(ability, payload2);

            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(21, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiySequence()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            Ability ability1 = abilitySystem.InstantiateAbility(CustomAbility.ATTACK_DECREASE);
            Ability ability2 = abilitySystem.InstantiateAbility(CustomAbility.NORAML_ATTACK);
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

            abilitySystem.TryEnqueueAndRunAbility(ability1, payload1);
            abilitySystem.TryEnqueueAndRunAbility(ability2, payload1);
            abilitySystem.TryEnqueueAndRunAbility(ability2, payload2);

            Assert.AreEqual(2, unit2.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(4, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(23, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ConditionalEntryNode_WithConditionSuccess_ExecuteAsExpected()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { name = "Mob1", });

            Ability ability = unit.AppendAbility(CustomAbility.LOG_WHEN_ATTACKED);
            var context = new CustomDamageEvent { target = unit, };
            abilitySystem.TryEnqueueAndRunAbility(ability, context);

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

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.NORAML_ATTACK_SELECTION);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.TryEnqueueAndRunAbility(ability, payload);

            Assert.IsNotNull(choiceContext);
            Assert.AreEqual(6, unit2.Owner.GetStat(CustomStats.HEALTH).CurrentValue);  // Damage should not occur
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveValidAnswer_EffectOccurred()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.NORAML_ATTACK_SELECTION);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.TryEnqueueAndRunAbility(ability, payload);

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

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.NORAML_ATTACK_SELECTION);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.TryEnqueueAndRunAbility(ability, payload);

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

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.NORAML_ATTACK_SELECTION);
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            abilitySystem.TryEnqueueAndRunAbility(ability, payload);

            Assert.IsNotNull(choiceContext);

            abilitySystem.Resume(new CustomCancellation());

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

            _ = unit.AppendAbility(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);
            abilitySystem.RefreshStatsAndModifiers();

            Assert.AreEqual(1, unit.Owner.Modifiers.Count);
            Assert.AreEqual(3, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_NotReachCondition_ModifierNotAppendedAndStatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            _ = unit.AppendAbility(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);
            abilitySystem.RefreshStatsAndModifiers();

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
            _ = unit.AppendAbility(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);
            abilitySystem.RefreshStatsAndModifiers();

            Assert.AreEqual(1, unit.Owner.Modifiers.Count);
            Assert.AreEqual(3, unit.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit.Owner.GetStat(CustomStats.ATTACK).CurrentValue);

            unit.SetStat(CustomStats.HEALTH, 6);
            abilitySystem.RefreshStatsAndModifiers();

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
            _ = unit2.AppendAbility(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.NORAML_ATTACK);
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

            abilitySystem.TryEnqueueAndRunAbility(ability, payload1);
            abilitySystem.TryEnqueueAndRunAbility(ability, payload2);

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
            _ = unit2.AppendAbility(CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED);

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.NORAML_ATTACK);
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

            abilitySystem.TryEnqueueAndRunAbility(ability, payload1);
            abilitySystem.TryEnqueueAndRunAbility(ability, payload2);

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
            _ = unit2.AppendAbility(CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED);
            _ = unit2.AppendAbility(CustomAbility.COUNTER_ATTACK);

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.NORMAL_ATTACK_5_TIMES);
            var payload = new CustomNormalAttackPayload
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            abilitySystem.TryEnqueueAndRunAbility(ability, payload);

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

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.NORMAL_ATTACK_5_TIMES);
            var payload = new CustomNormalAttackPayload
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            abilitySystem.TryEnqueueAndRunAbility(ability, payload);
            Assert.AreEqual(5, unit1.Owner.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiy_Macro()
        {
            var macro = CustomAbility.HELLO_WORLD_MACRO;
            abilitySystem.LoadMacroGraph(macro.name, macro);

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.HELLO_WORLD_MACRO_CALLER);
            abilitySystem.TryEnqueueAndRunAbility(ability, null);

            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "end");
        }

        [Test]
        public void ExecuteAbilitiy_LoopMacro5Times()
        {
            var macro = CustomAbility.HELLO_WORLD_MACRO;
            abilitySystem.LoadMacroGraph(macro.name, macro);

            Ability ability = abilitySystem.InstantiateAbility(CustomAbility.HELLO_WORLD_MACRO_CALLER_5_TIMES);
            abilitySystem.TryEnqueueAndRunAbility(ability, null);

            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "end");
        }

        [Test]
        public void ExecuteAbilitiy_ThrowException_ShouldAbortImmediately()
        {
            Ability throwException = abilitySystem.InstantiateAbility(CustomAbility.THROW_EXCEPTION);
            Ability helloWorld = abilitySystem.InstantiateAbility(CustomAbility.HELLO_WORLD);

            _ = abilitySystem.TryEnqueueAbility(throwException, null);
            _ = abilitySystem.TryEnqueueAbility(helloWorld, null);
            abilitySystem.Run();

            LogAssert.Expect(LogType.Exception, "Exception: This is for testing");
            LogAssert.Expect(LogType.Log, "Hello");
            LogAssert.Expect(LogType.Log, "World!");
            LogAssert.NoUnexpectedReceived();
        }
    }
}
