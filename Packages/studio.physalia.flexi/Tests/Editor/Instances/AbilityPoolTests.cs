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
            var abilityData = new AbilityData();
            abilitySystem.CreateAbilityPool(abilityData, 10);

            AbilityPool pool = abilitySystem.GetAbilityPool(abilityData);
            Assert.AreEqual(10, pool.Size);
            Assert.AreEqual(0, pool.UsingCount);
        }

        [Test]
        public void GetAbility4Times_PoolSizeIs10_SizeReturns10AndUsingCountReturns4()
        {
            var abilityData = new AbilityData();
            abilitySystem.CreateAbilityPool(abilityData, 10);

            for (int i = 0; i < 4; i++)
            {
                _ = abilitySystem.GetAbility(abilityData);
            }

            AbilityPool pool = abilitySystem.GetAbilityPool(abilityData);
            Assert.AreEqual(10, pool.Size);
            Assert.AreEqual(4, pool.UsingCount);
        }

        [Test]
        public void GetAbility4TimesAndRelease2Times_PoolSizeIs10_SizeReturns10AndUsingCountReturns2()
        {
            var abilityData = new AbilityData();
            abilitySystem.CreateAbilityPool(abilityData, 10);

            var list = new List<Ability>();
            for (int i = 0; i < 4; i++)
            {
                Ability ability = abilitySystem.GetAbility(abilityData);
                list.Add(ability);
            }

            for (int i = 0; i < 2; i++)
            {
                abilitySystem.ReleaseAbility(list[i]);
            }

            AbilityPool pool = abilitySystem.GetAbilityPool(abilityData);
            Assert.AreEqual(10, pool.Size);
            Assert.AreEqual(2, pool.UsingCount);
        }

        [Test]
        public void GetAbility14Times_StartPoolSizeIs10_SizeReturns14AndUsingCountReturns14()
        {
            var abilityData = new AbilityData();
            abilitySystem.CreateAbilityPool(abilityData, 10);

            for (int i = 0; i < 14; i++)
            {
                _ = abilitySystem.GetAbility(abilityData);
            }

            AbilityPool pool = abilitySystem.GetAbilityPool(abilityData);
            Assert.AreEqual(14, pool.Size);
            Assert.AreEqual(14, pool.UsingCount);
        }

        [Test]
        public void GetAbilityAndRelease_UserDataIsSet_UserDataShouldBeNull()
        {
            var abilityData = new AbilityData();
            abilitySystem.CreateAbilityPool(abilityData, 1);

            Ability ability = abilitySystem.GetAbility(abilityData);
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
            var abilityData = new AbilityData();
            for (var i = 0; i < 3; i++)
            {
                string json = AbilityGraphUtility.Serialize(abilityGraph);
                abilityData.graphJsons.Add(json);
            }

            // Poolize the ability
            abilitySystem.CreateAbilityPool(abilityData, 1);

            // Get an ability, run it and return it
            Ability ability = abilitySystem.GetAbility(abilityData);
            abilitySystem.TryEnqueueAbility(ability, null);
            abilitySystem.Run();
            abilitySystem.ReleaseAbility(ability);

            // Assert
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                AbilityFlow flow = ability.Flows[i];
                Assert.AreEqual(false, flow.IsRunning());
                Assert.AreEqual(null, flow.Current);
            }
        }
    }
}
