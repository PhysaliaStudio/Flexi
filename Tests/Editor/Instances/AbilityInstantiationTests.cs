using NUnit.Framework;
using UnityEngine;

namespace Physalia.AbilityFramework.Tests
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
            abilityData.blackboard.Add(new BlackboardVariable());
            abilityData.blackboard.Add(new BlackboardVariable());
            abilityData.blackboard.Add(new BlackboardVariable());

            Ability ability = abilitySystem.InstantiateAbility(abilityData);
            Assert.AreEqual(3, ability.Blackboard.Count);
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
