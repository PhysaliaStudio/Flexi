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
            AbilityDataSource abilityDataSource = CustomAbility.ATTACK_DOUBLE;
            unit.AppendAbilityDataSource(abilityDataSource);

            bool success1 = abilitySystem.TryEnqueueAbility(unit, abilityDataSource, null);
            abilitySystem.Run();

            Assert.AreEqual(true, success1);
            Assert.AreEqual(4, unit.GetStat(CustomStats.ATTACK).CurrentValue);

            bool success2 = abilitySystem.TryEnqueueAbility(unit, abilityDataSource, null);
            abilitySystem.Run();

            Assert.AreEqual(true, success2);
            Assert.AreEqual(8, unit.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void RunAbilityInstance_InstanceCanDoTheSameThingAsOriginal()
        {
            AbilityDataSource ability = CustomAbility.HELLO_WORLD;
            _ = abilitySystem.TryEnqueueAbility(null, ability, null);
            abilitySystem.Run();

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

            AbilityDataSource ability = CustomAbility.NORAML_ATTACK;
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

            _ = abilitySystem.TryEnqueueAbility(null, ability, payload1);
            abilitySystem.Run();

            _ = abilitySystem.TryEnqueueAbility(null, ability, payload2);
            abilitySystem.Run();

            Assert.AreEqual(4, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(21, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiySequence()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityDataSource ability1 = CustomAbility.ATTACK_DECREASE;
            AbilityDataSource ability2 = CustomAbility.NORAML_ATTACK;
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

            _ = abilitySystem.TryEnqueueAbility(null, ability1, payload1);
            abilitySystem.Run();

            _ = abilitySystem.TryEnqueueAbility(null, ability2, payload1);
            abilitySystem.Run();

            _ = abilitySystem.TryEnqueueAbility(null, ability2, payload2);
            abilitySystem.Run();

            Assert.AreEqual(2, unit2.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(4, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(23, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ConditionalEntryNode_WithConditionSuccess_ExecuteAsExpected()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { name = "Mob1" });
            AbilityDataSource logWhenAttacked = CustomAbility.LOG_WHEN_ATTACKED;
            unit.AppendAbilityDataSource(logWhenAttacked);

            var context = new CustomDamageEvent { target = unit };
            bool success = abilitySystem.TryEnqueueAbility(unit, logWhenAttacked, context);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
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

            AbilityDataSource normalAttackSelection = CustomAbility.NORAML_ATTACK_SELECTION;
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            bool success = abilitySystem.TryEnqueueAbility(null, normalAttackSelection, payload);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.IsNotNull(choiceContext);
            Assert.AreEqual(6, unit2.GetStat(CustomStats.HEALTH).CurrentValue);  // Damage should not occur
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveValidAnswer_EffectOccurred()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityDataSource normalAttackSelection = CustomAbility.NORAML_ATTACK_SELECTION;
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            bool success = abilitySystem.TryEnqueueAbility(null, normalAttackSelection, payload);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.IsNotNull(choiceContext);

            var answerContext = new CustomSingleTargetAnswerContext { target = unit2 };
            abilitySystem.Resume(answerContext);

            Assert.AreEqual(4, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveInvalidAnswer_LogErrorAndEffectNotOccurred()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityDataSource normalAttackSelection = CustomAbility.NORAML_ATTACK_SELECTION;
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            bool success = abilitySystem.TryEnqueueAbility(null, normalAttackSelection, payload);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.IsNotNull(choiceContext);

            var answerContext = new CustomSingleTargetAnswerContext { target = null };
            abilitySystem.Resume(answerContext);

            TestUtilities.LogAssertAnyString(LogType.Error);
            Assert.AreEqual(6, unit2.GetStat(CustomStats.HEALTH).CurrentValue);  // Damage should not occur
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveCancellation()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityDataSource normalAttackSelection = CustomAbility.NORAML_ATTACK_SELECTION;
            var payload = new CustomActivationPayload { activator = unit1 };

            IChoiceContext choiceContext = null;
            abilitySystem.ChoiceOccurred += context => choiceContext = context;

            bool success = abilitySystem.TryEnqueueAbility(null, normalAttackSelection, payload);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.IsNotNull(choiceContext);

            abilitySystem.Resume(new CustomCancellation());

            // Nothing happened
            Assert.AreEqual(25, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachCondition_ModifierAppendedAndStatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            unit.GetStat(CustomStats.HEALTH).CurrentBase = 3;

            unit.AppendAbilityDataSource(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);
            abilitySystem.RefreshStatsAndModifiers();

            Assert.AreEqual(1, unit.Modifiers.Count);
            Assert.AreEqual(3, unit.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_NotReachCondition_ModifierNotAppendedAndStatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            unit.AppendAbilityDataSource(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);
            abilitySystem.RefreshStatsAndModifiers();

            Assert.AreEqual(0, unit.Modifiers.Count);
            Assert.AreEqual(6, unit.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(4, unit.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachConditionThenMakeNotReach_ModifierRemovedAndStatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            unit.GetStat(CustomStats.HEALTH).CurrentBase = 3;
            unit.AppendAbilityDataSource(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);
            abilitySystem.RefreshStatsAndModifiers();

            Assert.AreEqual(1, unit.Modifiers.Count);
            Assert.AreEqual(3, unit.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit.GetStat(CustomStats.ATTACK).CurrentValue);

            unit.GetStat(CustomStats.HEALTH).CurrentBase = 6;
            abilitySystem.RefreshStatsAndModifiers();

            Assert.AreEqual(0, unit.Modifiers.Count);
            Assert.AreEqual(6, unit.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(4, unit.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachConditionWhileRunningSystem_ModifierNotAppendedAndStatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            unit2.AppendAbilityDataSource(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH);

            AbilityDataSource normalAttack = CustomAbility.NORAML_ATTACK;
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

            bool success1 = abilitySystem.TryEnqueueAbility(null, normalAttack, payload1);
            abilitySystem.Run();

            bool success2 = abilitySystem.TryEnqueueAbility(null, normalAttack, payload2);
            abilitySystem.Run();

            Assert.AreEqual(true, success1);
            Assert.AreEqual(true, success2);
            Assert.AreEqual(1, unit2.Modifiers.Count);
            Assert.AreEqual(3, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit2.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(19, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ContinuousEventFor2Times_TheAbilityTriggeredTwice()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityDataSource source = CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED;
            unit2.AppendAbilityDataSource(source);

            for (var i = 0; i < 2; i++)
            {
                abilitySystem.TryEnqueueAbility(unit2, source, new CustomDamageEvent
                {
                    instigator = unit1,
                    target = unit2,
                });
            }
            abilitySystem.Run();

            Assert.AreEqual(16, unit2.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ChainEffect_TriggerAnotherAbilityFromNodeByEvent_StatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });
            unit2.AppendAbilityDataSource(CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED);

            AbilityDataSource normalAttack = CustomAbility.NORAML_ATTACK;
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

            bool success1 = abilitySystem.TryEnqueueAbility(null, normalAttack, payload1);
            abilitySystem.Run();

            bool success2 = abilitySystem.TryEnqueueAbility(null, normalAttack, payload2);
            abilitySystem.Run();

            Assert.AreEqual(true, success1);
            Assert.AreEqual(true, success2);
            Assert.AreEqual(3, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(8, unit2.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(17, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ChainEffect_MultipleAbilities_TriggeredByCorrectOrder()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 64, attack = 1, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 10, attack = 1, });
            unit2.AppendAbilityDataSource(CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED);
            unit2.AppendAbilityDataSource(CustomAbility.COUNTER_ATTACK);

            AbilityDataSource normalAttack5Times = CustomAbility.NORMAL_ATTACK_5_TIMES;
            var payload = new CustomNormalAttackPayload
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            bool success = abilitySystem.TryEnqueueAbility(null, normalAttack5Times, payload);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.AreEqual(2, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(5, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(32, unit2.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiy_ForLoop_StatsAreCorrect()
        {
            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit1 = unitFactory.Create(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = unitFactory.Create(new CustomUnitData { health = 6, attack = 4, });

            AbilityDataSource source = CustomAbility.NORMAL_ATTACK_5_TIMES;
            var payload = new CustomNormalAttackPayload
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            _ = abilitySystem.TryEnqueueAbility(null, source, payload);
            abilitySystem.Run();

            Assert.AreEqual(5, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiy_Macro()
        {
            var macro = CustomAbility.HELLO_WORLD_MACRO;
            abilitySystem.LoadMacroGraph(macro.name, macro);

            AbilityDataSource source = CustomAbility.HELLO_WORLD_MACRO_CALLER;
            _ = abilitySystem.TryEnqueueAbility(null, source, null);
            abilitySystem.Run();

            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "end");
        }

        [Test]
        public void ExecuteAbilitiy_LoopMacro5Times()
        {
            var macro = CustomAbility.HELLO_WORLD_MACRO;
            abilitySystem.LoadMacroGraph(macro.name, macro);

            AbilityDataSource source = CustomAbility.HELLO_WORLD_MACRO_CALLER_5_TIMES;
            _ = abilitySystem.TryEnqueueAbility(null, source, null);
            abilitySystem.Run();

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
            AbilityDataSource throwException = CustomAbility.THROW_EXCEPTION;
            AbilityDataSource helloWorld = CustomAbility.HELLO_WORLD;

            _ = abilitySystem.TryEnqueueAbility(null, throwException, null);
            _ = abilitySystem.TryEnqueueAbility(null, helloWorld, null);
            abilitySystem.Run();

            LogAssert.Expect(LogType.Exception, "Exception: This is for testing");
            LogAssert.Expect(LogType.Log, "Hello");
            LogAssert.Expect(LogType.Log, "World!");
            LogAssert.NoUnexpectedReceived();
        }
    }
}
