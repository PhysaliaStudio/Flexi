using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.Flexi.Tests
{
    public class IntegrationTests
    {
        private AbilitySystemWrapperDefault wrapper;
        private AbilitySystem abilitySystem;
        private CustomUnitFactory unitFactory;

        private DefaultAbilityContainer CreateAbilityContainer(AbilityHandle handle)
        {
            abilitySystem.CreateAbilityPool(handle, 2);
            return new DefaultAbilityContainer { SystemWrapper = wrapper, Handle = handle };
        }

        private CustomUnit CreateUnit(CustomUnitData data)
        {
            CustomUnit unit = unitFactory.Create(data);
            wrapper.AppendActor(unit);
            return unit;
        }

        [SetUp]
        public void SetUp()
        {
            wrapper = new AbilitySystemWrapperDefault();
            AbilitySystemBuilder builder = new AbilitySystemBuilder();
            builder.SetWrapper(wrapper);
            abilitySystem = builder.Build();

            unitFactory = new CustomUnitFactory();
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
            CustomUnit unit = CreateUnit(new CustomUnitData { health = 25, attack = 2, });
            DefaultAbilityContainer container = CreateAbilityContainer(CustomAbility.ATTACK_DOUBLE);
            unit.AppendAbilityContainer(container);

            bool success1 = abilitySystem.TryEnqueueAbility(container);
            abilitySystem.Run();

            Assert.AreEqual(true, success1);
            Assert.AreEqual(4, unit.GetStat(CustomStats.ATTACK).CurrentValue);

            bool success2 = abilitySystem.TryEnqueueAbility(container);
            abilitySystem.Run();

            Assert.AreEqual(true, success2);
            Assert.AreEqual(8, unit.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void RunAbilityInstance_InstanceCanDoTheSameThingAsOriginal()
        {
            DefaultAbilityContainer container = CreateAbilityContainer(CustomAbility.HELLO_WORLD);
            _ = abilitySystem.TryEnqueueAbility(container);
            abilitySystem.Run();

            // Check if the instance can do the same thing
            LogAssert.Expect(LogType.Log, "Hello");
            LogAssert.Expect(LogType.Log, "World!");
        }

        [Test]
        public void ExecuteCustomNodesAndAbilitiy()
        {
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });

            DefaultAbilityContainer container = CreateAbilityContainer(CustomAbility.NORAML_ATTACK);
            var context1 = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            var context2 = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            _ = abilitySystem.TryEnqueueAbility(container, context1);
            abilitySystem.Run();

            _ = abilitySystem.TryEnqueueAbility(container, context2);
            abilitySystem.Run();

            Assert.AreEqual(4, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(21, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiySequence()
        {
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });

            DefaultAbilityContainer container1 = CreateAbilityContainer(CustomAbility.ATTACK_DECREASE);
            DefaultAbilityContainer container2 = CreateAbilityContainer(CustomAbility.NORAML_ATTACK);
            var context1 = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            var context2 = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            _ = abilitySystem.TryEnqueueAbility(container1, context1);
            abilitySystem.Run();

            _ = abilitySystem.TryEnqueueAbility(container2, context1);
            abilitySystem.Run();

            _ = abilitySystem.TryEnqueueAbility(container2, context2);
            abilitySystem.Run();

            Assert.AreEqual(2, unit2.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(4, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(23, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ConditionalEntryNode_WithConditionSuccess_ExecuteAsExpected()
        {
            CustomUnit unit = CreateUnit(new CustomUnitData { name = "Mob1" });
            DefaultAbilityContainer container = CreateAbilityContainer(CustomAbility.LOG_WHEN_ATTACKED);
            unit.AppendAbilityContainer(container);

            var context = new CustomDamageEventNode.Context { target = unit };
            bool success = abilitySystem.TryEnqueueAbility(container, context);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            LogAssert.Expect(LogType.Log, "I'm damaged!");
            LogAssert.Expect(LogType.Log, "I will revenge!");
        }

        [Test]
        public void TargetSelectionAbilitiy_ReceivesChoice()
        {
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });

            DefaultAbilityContainer container = CreateAbilityContainer(CustomAbility.NORAML_ATTACK_SELECTION);
            var context = new CustomActivationNode.Context { activator = unit1 };

            bool choiceTriggered = false;
            wrapper.ChoiceTriggered += () => choiceTriggered = true;

            bool success = abilitySystem.TryEnqueueAbility(container, context);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.AreEqual(true, choiceTriggered);
            Assert.AreEqual(6, unit2.GetStat(CustomStats.HEALTH).CurrentValue);  // Damage should not occur
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveValidAnswer_EffectOccurred()
        {
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });

            DefaultAbilityContainer container = CreateAbilityContainer(CustomAbility.NORAML_ATTACK_SELECTION);
            var context = new CustomActivationNode.Context { activator = unit1 };

            bool choiceTriggered = false;
            wrapper.ChoiceTriggered += () => choiceTriggered = true;

            bool success = abilitySystem.TryEnqueueAbility(container, context);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.AreEqual(true, choiceTriggered);

            var answerContext = new CustomSingleTargetAnswerContext { target = unit2 };
            abilitySystem.Resume(answerContext);

            Assert.AreEqual(4, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveInvalidAnswer_LogErrorAndEffectNotOccurred()
        {
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });

            DefaultAbilityContainer container = CreateAbilityContainer(CustomAbility.NORAML_ATTACK_SELECTION);
            var context = new CustomActivationNode.Context { activator = unit1 };

            bool choiceTriggered = false;
            wrapper.ChoiceTriggered += () => choiceTriggered = true;

            bool success = abilitySystem.TryEnqueueAbility(container, context);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.AreEqual(true, choiceTriggered);

            var answerContext = new CustomSingleTargetAnswerContext { target = null };
            abilitySystem.Resume(answerContext);

            TestUtilities.LogAssertAnyString(LogType.Error);
            Assert.AreEqual(6, unit2.GetStat(CustomStats.HEALTH).CurrentValue);  // Damage should not occur
        }

        [Test]
        public void TargetSelectionAbilitiy_GiveCancellation()
        {
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 2, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });

            DefaultAbilityContainer container = CreateAbilityContainer(CustomAbility.NORAML_ATTACK_SELECTION);
            var context = new CustomActivationNode.Context { activator = unit1 };

            bool choiceTriggered = false;
            wrapper.ChoiceTriggered += () => choiceTriggered = true;

            bool success = abilitySystem.TryEnqueueAbility(container, context);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.AreEqual(true, choiceTriggered);

            abilitySystem.Resume(new CustomCancellation());

            // Nothing happened
            Assert.AreEqual(25, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachCondition_ModifierAppendedAndStatsAreCorrect()
        {
            CustomUnit unit = CreateUnit(new CustomUnitData { health = 6, attack = 4, });
            unit.GetStat(CustomStats.HEALTH).CurrentBase = 3;

            unit.AppendAbilityContainer(CreateAbilityContainer(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH));
            abilitySystem.RefreshStatsAndModifiers();

            Assert.AreEqual(1, unit.Modifiers.Count);
            Assert.AreEqual(3, unit.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_NotReachCondition_ModifierNotAppendedAndStatsAreCorrect()
        {
            CustomUnit unit = CreateUnit(new CustomUnitData { health = 6, attack = 4, });

            unit.AppendAbilityContainer(CreateAbilityContainer(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH));
            abilitySystem.RefreshStatsAndModifiers();

            Assert.AreEqual(0, unit.Modifiers.Count);
            Assert.AreEqual(6, unit.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(4, unit.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ConditionalModifier_ReachConditionThenMakeNotReach_ModifierRemovedAndStatsAreCorrect()
        {
            CustomUnit unit = CreateUnit(new CustomUnitData { health = 6, attack = 4, });
            unit.GetStat(CustomStats.HEALTH).CurrentBase = 3;
            unit.AppendAbilityContainer(CreateAbilityContainer(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH));
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
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });
            unit2.AppendAbilityContainer(CreateAbilityContainer(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH));

            DefaultAbilityContainer normalAttack = CreateAbilityContainer(CustomAbility.NORAML_ATTACK);
            var context1 = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            var context2 = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            bool success1 = abilitySystem.TryEnqueueAbility(normalAttack, context1);
            abilitySystem.Run();

            bool success2 = abilitySystem.TryEnqueueAbility(normalAttack, context2);
            abilitySystem.Run();

            Assert.AreEqual(true, success1);
            Assert.AreEqual(true, success2);
            Assert.AreEqual(1, unit2.Modifiers.Count);
            Assert.AreEqual(3, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(6, unit2.GetStat(CustomStats.ATTACK).CurrentValue);
            Assert.AreEqual(19, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void MultiOrderModifiers_ReachCondition_ModifierAppendedAndStatsAreCorrect()
        {
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });
            // Note: We intentionally add the modifier with the reverse order for testing.
            unit2.AppendAbilityContainer(CreateAbilityContainer(CustomAbility.ATTACK_DOUBLE_WHEN_GREATER_THAN_5));
            unit2.AppendAbilityContainer(CreateAbilityContainer(CustomAbility.ATTACK_UP_WHEN_LOW_HEALTH));

            DefaultAbilityContainer normalAttack = CreateAbilityContainer(CustomAbility.NORAML_ATTACK);
            var context = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            bool success = abilitySystem.TryEnqueueAbility(normalAttack, context);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.AreEqual(2, unit2.Modifiers.Count);
            Assert.AreEqual(3, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(12, unit2.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ContinuousEventFor2Times_TheAbilityTriggeredTwice()
        {
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });
            DefaultAbilityContainer container = CreateAbilityContainer(CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED);
            unit2.AppendAbilityContainer(container);

            for (var i = 0; i < 2; i++)
            {
                abilitySystem.TryEnqueueAbility(container, new CustomDamageEventNode.Context
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
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });
            unit2.AppendAbilityContainer(CreateAbilityContainer(CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED));

            DefaultAbilityContainer normalAttack = CreateAbilityContainer(CustomAbility.NORAML_ATTACK);
            var context1 = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            var context2 = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            bool success1 = abilitySystem.TryEnqueueAbility(normalAttack, context1);
            abilitySystem.Run();

            bool success2 = abilitySystem.TryEnqueueAbility(normalAttack, context2);
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
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 64, attack = 1, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 10, attack = 1, });
            unit2.AppendAbilityContainer(CreateAbilityContainer(CustomAbility.ATTACK_DOUBLE_WHEN_DAMAGED));
            unit2.AppendAbilityContainer(CreateAbilityContainer(CustomAbility.COUNTER_ATTACK));

            DefaultAbilityContainer normalAttack5Times = CreateAbilityContainer(CustomAbility.NORMAL_ATTACK_5_TIMES);
            var context = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit1,
                mainTarget = unit2,
            };

            bool success = abilitySystem.TryEnqueueAbility(normalAttack5Times, context);
            abilitySystem.Run();

            Assert.AreEqual(true, success);
            Assert.AreEqual(2, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(5, unit2.GetStat(CustomStats.HEALTH).CurrentValue);
            Assert.AreEqual(32, unit2.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiy_ForLoop_StatsAreCorrect()
        {
            CustomUnit unit1 = CreateUnit(new CustomUnitData { health = 25, attack = 3, });
            CustomUnit unit2 = CreateUnit(new CustomUnitData { health = 6, attack = 4, });

            DefaultAbilityContainer normalAttack5Times = CreateAbilityContainer(CustomAbility.NORMAL_ATTACK_5_TIMES);
            var context = new CustomNormalAttackEntryNode.Context
            {
                attacker = unit2,
                mainTarget = unit1,
            };

            _ = abilitySystem.TryEnqueueAbility(normalAttack5Times, context);
            abilitySystem.Run();

            Assert.AreEqual(5, unit1.GetStat(CustomStats.HEALTH).CurrentValue);
        }

        [Test]
        public void ExecuteAbilitiy_Macro()
        {
            var macro = CustomAbility.HELLO_WORLD_MACRO;
            abilitySystem.LoadMacroGraph(macro.name, macro);

            DefaultAbilityContainer source = CreateAbilityContainer(CustomAbility.HELLO_WORLD_MACRO_CALLER);
            _ = abilitySystem.TryEnqueueAbility(source);
            abilitySystem.Run();

            LogAssert.Expect(LogType.Log, "Hello World!");
            LogAssert.Expect(LogType.Log, "end");
        }

        [Test]
        public void ExecuteAbilitiy_LoopMacro5Times()
        {
            var macro = CustomAbility.HELLO_WORLD_MACRO;
            abilitySystem.LoadMacroGraph(macro.name, macro);

            DefaultAbilityContainer source = CreateAbilityContainer(CustomAbility.HELLO_WORLD_MACRO_CALLER_5_TIMES);
            _ = abilitySystem.TryEnqueueAbility(source);
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
            DefaultAbilityContainer throwException = CreateAbilityContainer(CustomAbility.THROW_EXCEPTION);
            DefaultAbilityContainer helloWorld = CreateAbilityContainer(CustomAbility.HELLO_WORLD);

            _ = abilitySystem.TryEnqueueAbility(throwException);
            _ = abilitySystem.TryEnqueueAbility(helloWorld);
            abilitySystem.Run();

            LogAssert.Expect(LogType.Exception, "Exception: This is for testing");
            LogAssert.Expect(LogType.Log, "Hello");
            LogAssert.Expect(LogType.Log, "World!");
        }

        [Test]
        public void ExecuteAbilitiy_AbilityIsPoolized_NormallyFinished_AbilitiesShouldBeReleased()
        {
            AbilityHandle helloWorld = CustomAbility.HELLO_WORLD;
            abilitySystem.CreateAbilityPool(helloWorld, 4);

            var container = new DefaultAbilityContainer { Handle = helloWorld };
            _ = abilitySystem.TryEnqueueAbility(container);
            abilitySystem.Run();

            Assert.AreEqual(0, abilitySystem.GetAbilityPool(helloWorld).UsingCount);
        }
    }
}
