using NUnit.Framework;
using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public class AbilityInstantiationTests
    {
        private AbilitySystem abilitySystem;

        [SetUp]
        public void SetUp()
        {
            AbilitySystemBuilder builder = new AbilitySystemBuilder();
            builder.SetWrapper(new AbilitySystemWrapperMock());
            abilitySystem = builder.Build();
        }

        [Test]
        public void InstantiateAbility_SystemReturnsTheAbilitySystem()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();

            Assert.AreEqual(abilitySystem, ability.System);
        }

        [Test]
        public void InstantiateAbility_DataReturnsTheSourceData()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();

            Assert.AreEqual(abilityHandle, ability.Handle);
            Assert.AreEqual(abilityHandle.Data, ability.Data);
        }

        [Test]
        public void InstantiateAbility_Contains3Variables_VariableCountIs3()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = "a" });
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = "b" });
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = "c" });

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();

            Assert.AreEqual(3, ability.Blackboard.Count);
        }

        [Test]
        public void InstantiateAbility_ContainsVariableWithEmptyKey_LogWarning()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            abilityHandle.Data.blackboard.Add(new BlackboardVariable());

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            _ = abilityFactory.Create();

            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_ContainsVariableWithWhiteSpaceKey_LogWarning()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = " " });

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            _ = abilityFactory.Create();

            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_ContainsVariablesA5AndA3_LogWarning()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = "A", value = 5 });
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = "A", value = 3 });

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            _ = abilityFactory.Create();

            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_ContainsVariablesA5AndA3_GetVariableWithAReturns5()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = "A", value = 5 });
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = "A", value = 3 });

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();

            Assert.AreEqual(5, ability.GetVariable("A"));
        }

        [Test]
        public void GetVariableWithA_ContainsVariableA42_Returns42()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = "A", value = 42 });

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();

            Assert.AreEqual(42, ability.GetVariable("A"));
        }

        [Test]
        public void GetVariableWithA_NoSuchKey_Returns0AndLogWarning()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();

            Assert.AreEqual(0, ability.GetVariable("A"));
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void OverrideVariableAWith99_TheOriginalValueIs42_GetVariableWithAReturns99()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            abilityHandle.Data.blackboard.Add(new BlackboardVariable { key = "A", value = 42 });

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();
            ability.OverrideVariable("A", 99);

            Assert.AreEqual(99, ability.GetVariable("A"));
        }

        [Test]
        public void OverrideVariableAWith99_NoMatchKey_GetVariableWithAReturns99AndLogWarning()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();
            ability.OverrideVariable("A", 99);

            Assert.AreEqual(99, ability.GetVariable("A"));
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_Contains2GraphJsons_FlowCountIs2()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            AbilityTestHelper.AppendGraphToSource(abilityHandle, "");
            AbilityTestHelper.AppendGraphToSource(abilityHandle, "");

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();

            Assert.AreEqual(2, ability.Flows.Count);
        }

        [Test]
        public void InstantiateAbility_AbilityOfEachFlowReturnsTheSourceAbility()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            AbilityTestHelper.AppendGraphToSource(abilityHandle, "");
            AbilityTestHelper.AppendGraphToSource(abilityHandle, "");

            var abilityFactory = new AbilityFactory(abilitySystem, abilityHandle);
            Ability ability = abilityFactory.Create();

            for (var i = 0; i < ability.Flows.Count; i++)
            {
                Assert.AreEqual(ability, ability.Flows[i].Ability);
            }
        }
    }
}
