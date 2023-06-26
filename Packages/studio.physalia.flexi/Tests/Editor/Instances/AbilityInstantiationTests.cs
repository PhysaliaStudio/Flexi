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

            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            statDefinitionListAsset.stats.AddRange(CustomStats.List);
            builder.SetStatDefinitions(statDefinitionListAsset);

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

        [Test]
        public void AppendAbility_FindAbilityWithSourceDataReturnsTheAbility()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            Ability ability = unit.AppendAbility(abilityDataSource);

            Assert.AreEqual(ability, unit.FindAbility(abilityDataSource));
        }

        [Test]
        public void AppendAbility_ActorOfAbilityReturnsTheSourceActor()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            Ability ability = unit.AppendAbility(abilityDataSource);

            Assert.AreEqual(unit, ability.Actor);
        }

        [Test]
        public void AppendAbility_Contains3GraphJsons_ActorHas3Flows()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            unit.AppendAbility(abilityDataSource);

            Assert.AreEqual(3, unit.AbilityFlows.Count);
        }


        [Test]
        public void RemoveAbility_ActorHasThatAbility_ReturnsTrue()
        {
            // We should test 0 flow case, because the returned value should be true even if there's no flow.
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            unit.AppendAbility(abilityDataSource);

            bool success = unit.RemoveAbility(abilityDataSource);
            Assert.AreEqual(true, success);
        }

        [Test]
        public void RemoveAbility_ActorDoesNotHaveThatAbility_ReturnsFalse()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());

            bool success = unit.RemoveAbility(abilityDataSource);
            Assert.AreEqual(false, success);
        }

        [Test]
        public void RemoveAbility_ActorHasThatAbility_FindAbilityWithSourceDataReturnsNull()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            unit.AppendAbility(abilityDataSource);

            unit.RemoveAbility(abilityDataSource);
            Assert.AreEqual(null, unit.FindAbility(abilityDataSource));
        }

        [Test]
        public void RemoveAbility_ActorHaveAnAbilityWith3Graphs_ActorHas0Flows()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");
            AbilityTestHelper.AppendGraphToSource(abilityDataSource, "");

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            unit.AppendAbility(abilityDataSource);

            unit.RemoveAbility(abilityDataSource);

            Assert.AreEqual(0, unit.AbilityFlows.Count);
        }
    }
}
