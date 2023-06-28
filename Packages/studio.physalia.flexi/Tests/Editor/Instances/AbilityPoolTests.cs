using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public class AbilityPoolTests
    {
        private AbilitySystem abilitySystem;

        [SetUp]
        public void SetUp()
        {
            AbilitySystemBuilder builder = new AbilitySystemBuilder();

            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            builder.SetStatDefinitions(statDefinitionListAsset);

            abilitySystem = builder.Build();
        }

        [Test]
        public void CreatePoolWithSize10_SizeReturns10()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilitySystem.CreateAbilityPool(abilityDataSource, 10);

            AbilityPool pool = abilitySystem.GetAbilityPool(abilityDataSource);
            Assert.AreEqual(10, pool.Size);
            Assert.AreEqual(0, pool.UsingCount);
        }

        [Test]
        public void GetAbility4Times_PoolSizeIs10_SizeReturns10AndUsingCountReturns4()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilitySystem.CreateAbilityPool(abilityDataSource, 10);

            for (int i = 0; i < 4; i++)
            {
                _ = abilitySystem.GetAbility(abilityDataSource);
            }

            AbilityPool pool = abilitySystem.GetAbilityPool(abilityDataSource);
            Assert.AreEqual(10, pool.Size);
            Assert.AreEqual(4, pool.UsingCount);
        }

        [Test]
        public void GetAbility4TimesAndRelease2Times_PoolSizeIs10_SizeReturns10AndUsingCountReturns2()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilitySystem.CreateAbilityPool(abilityDataSource, 10);

            var list = new List<Ability>();
            for (int i = 0; i < 4; i++)
            {
                Ability ability = abilitySystem.GetAbility(abilityDataSource);
                list.Add(ability);
            }

            for (int i = 0; i < 2; i++)
            {
                abilitySystem.ReleaseAbility(list[i]);
            }

            AbilityPool pool = abilitySystem.GetAbilityPool(abilityDataSource);
            Assert.AreEqual(10, pool.Size);
            Assert.AreEqual(2, pool.UsingCount);
        }

        [Test]
        public void GetAbility14Times_StartPoolSizeIs10_SizeReturns14AndUsingCountReturns14()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilitySystem.CreateAbilityPool(abilityDataSource, 10);

            for (int i = 0; i < 14; i++)
            {
                _ = abilitySystem.GetAbility(abilityDataSource);
            }

            AbilityPool pool = abilitySystem.GetAbilityPool(abilityDataSource);
            Assert.AreEqual(14, pool.Size);
            Assert.AreEqual(14, pool.UsingCount);
        }

        [Test]
        public void GetAbilityAndRelease_UserDataIsSet_UserDataShouldBeNull()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilitySystem.CreateAbilityPool(abilityDataSource, 1);

            Ability ability = abilitySystem.GetAbility(abilityDataSource);
            ability.SetUserData(new object());
            abilitySystem.ReleaseAbility(ability);

            Assert.AreEqual(null, ability.GetUserData<object>());
        }

        [Test]
        public void GetAbilityAndRelease_MakeFlowsRunning_EveryFlowsShouldBeNotRunning()
        {
            // Create an ability graph that can pause
            var abilityGraph = new AbilityGraph();
            StartNode startNode = abilityGraph.AddNewNode<StartNode>();
            PauseNode pauseNode = abilityGraph.AddNewNode<PauseNode>();
            startNode.next.Connect(pauseNode.previous);

            // Create an ability data with 3 flows
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            string json = AbilityGraphUtility.Serialize(abilityGraph);
            for (var i = 0; i < 3; i++)
            {
                AbilityTestHelper.AppendGraphToSource(abilityDataSource, json);
            }

            // Poolize the ability
            abilitySystem.CreateAbilityPool(abilityDataSource, 1);

            // Get an ability, run it and return it
            Ability ability = abilitySystem.GetAbility(abilityDataSource);
            abilitySystem.TryEnqueueAbility(ability, null);
            abilitySystem.Run();
            abilitySystem.ReleaseAbility(ability);

            // Assert
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                AbilityFlow flow = ability.Flows[i];
                Assert.AreEqual(null, flow.Current);
            }
        }

        [Test]
        public void AppendPoolizedAbilityToActor_UsingCountIs1()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilitySystem.CreateAbilityPool(abilityDataSource, 1);

            var actor = new EmptyActor(abilitySystem);
            _ = actor.AppendAbility(abilityDataSource);

            Assert.AreEqual(1, abilitySystem.GetAbilityPool(abilityDataSource).UsingCount);
        }

        [Test]
        public void AppendPoolizedAbilityToActorAndRelease_UsingCountIs0()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilitySystem.CreateAbilityPool(abilityDataSource, 1);

            var actor = new EmptyActor(abilitySystem);
            Ability ability = actor.AppendAbility(abilityDataSource);
            actor.RemoveAbility(ability);

            Assert.AreEqual(0, abilitySystem.GetAbilityPool(abilityDataSource).UsingCount);
        }

        [Test]
        public void AppendPoolizedAbilityToActorAndRelease_ActorCachesShouldBeNull()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            abilitySystem.CreateAbilityPool(abilityDataSource, 1);

            var actor = new EmptyActor(abilitySystem);
            Ability ability = actor.AppendAbility(abilityDataSource);
            actor.RemoveAbility(ability);

            Assert.AreEqual(null, ability.Actor);
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                Assert.AreEqual(null, ability.Flows[i].Actor);
            }
        }
    }
}
