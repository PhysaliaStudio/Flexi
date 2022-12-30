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
            var abilityData = new AbilityData();
            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            Assert.AreEqual(abilitySystem, ability.System);
        }

        [Test]
        public void InstantiateAbility_DataReturnsTheSourceData()
        {
            var abilityData = new AbilityData();
            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            Assert.AreEqual(abilityData, ability.Data);
        }

        [Test]
        public void InstantiateAbility_Contains3Variables_VariableCountIs3()
        {
            var abilityData = new AbilityData();
            abilityData.blackboard.Add(new BlackboardVariable { key = "a" });
            abilityData.blackboard.Add(new BlackboardVariable { key = "b" });
            abilityData.blackboard.Add(new BlackboardVariable { key = "c" });

            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            Assert.AreEqual(3, ability.Blackboard.Count);
        }

        [Test]
        public void InstantiateAbility_ContainsVariableWithEmptyKey_LogWarning()
        {
            var abilityData = new AbilityData();
            abilityData.blackboard.Add(new BlackboardVariable());

            _ = abilitySystem.InstantiateAbility(abilityData);
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_ContainsVariableWithWhiteSpaceKey_LogWarning()
        {
            var abilityData = new AbilityData();
            abilityData.blackboard.Add(new BlackboardVariable { key = " " });

            _ = abilitySystem.InstantiateAbility(abilityData);
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_ContainsVariablesA5AndA3_LogWarning()
        {
            var abilityData = new AbilityData();
            abilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 5 });
            abilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 3 });

            _ = abilitySystem.InstantiateAbility(abilityData);
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_ContainsVariablesA5AndA3_GetVariableWithAReturns5()
        {
            var abilityData = new AbilityData();
            abilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 5 });
            abilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 3 });

            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            Assert.AreEqual(5, ability.GetVariable("A"));
        }

        [Test]
        public void GetVariableWithA_ContainsVariableA42_Returns42()
        {
            var abilityData = new AbilityData();
            abilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 42 });

            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            Assert.AreEqual(42, ability.GetVariable("A"));
        }

        [Test]
        public void GetVariableWithA_NoSuchKey_Returns0AndLogWarning()
        {
            var abilityData = new AbilityData();
            Ability ability = abilitySystem.InstantiateAbility(abilityData);

            Assert.AreEqual(0, ability.GetVariable("A"));
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void OverrideVariableAWith99_TheOriginalValueIs42_GetVariableWithAReturns99()
        {
            var abilityData = new AbilityData();
            abilityData.blackboard.Add(new BlackboardVariable { key = "A", value = 42 });

            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            ability.OverrideVariable("A", 99);

            Assert.AreEqual(99, ability.GetVariable("A"));
        }

        [Test]
        public void OverrideVariableAWith99_NoMatchKey_GetVariableWithAReturns99AndLogWarning()
        {
            var abilityData = new AbilityData();

            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            ability.OverrideVariable("A", 99);

            Assert.AreEqual(99, ability.GetVariable("A"));
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void InstantiateAbility_Contains2GraphJsons_FlowCountIs2()
        {
            var abilityData = new AbilityData();
            abilityData.graphJsons.Add("");
            abilityData.graphJsons.Add("");

            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            Assert.AreEqual(2, ability.Flows.Count);
        }

        [Test]
        public void InstantiateAbility_AbilityOfEachFlowReturnsTheSourceAbility()
        {
            var abilityData = new AbilityData();
            abilityData.graphJsons.Add("");
            abilityData.graphJsons.Add("");

            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                Assert.AreEqual(ability, ability.Flows[i].Ability);
            }
        }

        [Test]
        public void AppendAbility_FindAbilityWithSourceDataReturnsTheAbility()
        {
            var abilityData = new AbilityData();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            Ability ability = unit.AppendAbility(abilityData);

            Assert.AreEqual(ability, unit.FindAbility(abilityData));
        }

        [Test]
        public void AppendAbility_ActorOfAbilityReturnsTheSourceActor()
        {
            var abilityData = new AbilityData();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            Ability ability = unit.AppendAbility(abilityData);

            Assert.AreEqual(unit, ability.Actor);
        }

        [Test]
        public void AppendAbility_Contains3GraphJsons_ActorHas3Flows()
        {
            var abilityData = new AbilityData();
            abilityData.graphJsons.Add("");
            abilityData.graphJsons.Add("");
            abilityData.graphJsons.Add("");

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            unit.AppendAbility(abilityData);

            Assert.AreEqual(3, unit.AbilityFlows.Count);
        }


        [Test]
        public void RemoveAbility_ActorHasThatAbility_ReturnsTrue()
        {
            // We should test 0 flow case, because the returned value should be true even if there's no flow.
            var abilityData = new AbilityData();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            unit.AppendAbility(abilityData);

            bool success = unit.RemoveAbility(abilityData);
            Assert.AreEqual(true, success);
        }

        [Test]
        public void RemoveAbility_ActorDoesNotHaveThatAbility_ReturnsFalse()
        {
            var abilityData = new AbilityData();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());

            bool success = unit.RemoveAbility(abilityData);
            Assert.AreEqual(false, success);
        }

        [Test]
        public void RemoveAbility_ActorHasThatAbility_FindAbilityWithSourceDataReturnsNull()
        {
            var abilityData = new AbilityData();

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            unit.AppendAbility(abilityData);

            unit.RemoveAbility(abilityData);
            Assert.AreEqual(null, unit.FindAbility(abilityData));
        }

        [Test]
        public void RemoveAbility_ActorHaveAnAbilityWith3Graphs_ActorHas0Flows()
        {
            var abilityData = new AbilityData();
            abilityData.graphJsons.Add("");
            abilityData.graphJsons.Add("");
            abilityData.graphJsons.Add("");

            var unitFactory = new CustomUnitFactory(abilitySystem);
            CustomUnit unit = unitFactory.Create(new CustomUnitData());
            unit.AppendAbility(abilityData);

            unit.RemoveAbility(abilityData);

            Assert.AreEqual(0, unit.AbilityFlows.Count);
        }
    }
}
