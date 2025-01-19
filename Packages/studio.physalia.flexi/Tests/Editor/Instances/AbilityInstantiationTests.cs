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
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            Assert.AreEqual(abilitySystem, ability.System);
        }

        [Test]
        public void InstantiateAbility_DataReturnsTheSourceData()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            Assert.AreEqual(abilityDataSource, ability.DataSource);
            Assert.AreEqual(abilityDataSource.AbilityData, ability.Data);
        }

        [Test]
        public void InstantiateAbility_Contains3Variables_VariableCountIs3()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = "a" });
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = "b" });
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = "c" });

            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            Assert.AreEqual(3, ability.Blackboard.Count);
        }

        [Test]
        public void InstantiateAbility_ContainsVariableWithEmptyKey_LogWarning()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable());

            _ = abilitySystem.InstantiateAbility(abilityDataSource);
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_ContainsVariableWithWhiteSpaceKey_LogWarning()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = " " });

            _ = abilitySystem.InstantiateAbility(abilityDataSource);
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_ContainsVariablesA5AndA3_LogWarning()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 5 });
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 3 });

            _ = abilitySystem.InstantiateAbility(abilityDataSource);
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_ContainsVariablesA5AndA3_GetVariableWithAReturns5()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 5 });
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 3 });

            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            Assert.AreEqual(5, ability.GetVariable("A"));
        }

        [Test]
        public void GetVariableWithA_ContainsVariableA42_Returns42()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 42 });

            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            Assert.AreEqual(42, ability.GetVariable("A"));
        }

        [Test]
        public void GetVariableWithA_NoSuchKey_Returns0AndLogWarning()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);

            Assert.AreEqual(0, ability.GetVariable("A"));
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void OverrideVariableAWith99_TheOriginalValueIs42_GetVariableWithAReturns99()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilityDataSource.AbilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 42 });

            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            ability.OverrideVariable("A", 99);

            Assert.AreEqual(99, ability.GetVariable("A"));
        }

        [Test]
        public void OverrideVariableAWith99_NoMatchKey_GetVariableWithAReturns99AndLogWarning()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();

            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            ability.OverrideVariable("A", 99);

            Assert.AreEqual(99, ability.GetVariable("A"));
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_Contains2GraphJsons_FlowCountIs2()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");

            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            Assert.AreEqual(2, ability.Flows.Count);
        }

        [Test]
        public void InstantiateAbility_AbilityOfEachFlowReturnsTheSourceAbility()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");

            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                Assert.AreEqual(ability, ability.Flows[i].Ability);
            }
        }
    }
}
