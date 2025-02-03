using System.Collections.Generic;
using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class AbilityPoolTests
    {
        private FlexiCore flexiCore;

        [SetUp]
        public void SetUp()
        {
            FlexiCoreBuilder builder = new FlexiCoreBuilder();
            builder.SetEventResolver(new FlexiCoreWrapperMock());
            builder.SetStatRefreshResolver(new FlexiCoreWrapperMock());
            flexiCore = builder.Build();
        }

        [Test]
        public void CreatePoolWithSize10_SizeReturns10()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            flexiCore.LoadAbility(abilityHandle, 10);

            AbilityPool pool = flexiCore.GetAbilityPool(abilityHandle);
            Assert.AreEqual(10, pool.Size);
            Assert.AreEqual(0, pool.UsingCount);
        }

        [Test]
        public void GetAbility4Times_PoolSizeIs10_SizeReturns10AndUsingCountReturns4()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            flexiCore.LoadAbility(abilityHandle, 10);

            for (int i = 0; i < 4; i++)
            {
                _ = flexiCore.GetAbility(abilityHandle);
            }

            AbilityPool pool = flexiCore.GetAbilityPool(abilityHandle);
            Assert.AreEqual(10, pool.Size);
            Assert.AreEqual(4, pool.UsingCount);
        }

        [Test]
        public void GetAbility4TimesAndRelease2Times_PoolSizeIs10_SizeReturns10AndUsingCountReturns2()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            flexiCore.LoadAbility(abilityHandle, 10);

            var list = new List<Ability>();
            for (int i = 0; i < 4; i++)
            {
                Ability ability = flexiCore.GetAbility(abilityHandle);
                list.Add(ability);
            }

            for (int i = 0; i < 2; i++)
            {
                flexiCore.ReleaseAbility(list[i]);
            }

            AbilityPool pool = flexiCore.GetAbilityPool(abilityHandle);
            Assert.AreEqual(10, pool.Size);
            Assert.AreEqual(2, pool.UsingCount);
        }

        [Test]
        public void GetAbility14Times_StartPoolSizeIs10_SizeReturns14AndUsingCountReturns14()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            flexiCore.LoadAbility(abilityHandle, 10);

            for (int i = 0; i < 14; i++)
            {
                _ = flexiCore.GetAbility(abilityHandle);
            }

            AbilityPool pool = flexiCore.GetAbilityPool(abilityHandle);
            Assert.AreEqual(14, pool.Size);
            Assert.AreEqual(14, pool.UsingCount);
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
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            string json = AbilityGraphUtility.Serialize(abilityGraph);
            for (var i = 0; i < 3; i++)
            {
                AbilityTestHelper.AppendGraphToSource(abilityHandle, json);
            }

            // Poolize the ability
            flexiCore.LoadAbility(abilityHandle, 1);

            // Get an ability, manually run it and return it
            Ability ability = flexiCore.GetAbility(abilityHandle);
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                AbilityFlow flow = ability.Flows[i];
                flow.Reset(0);
                flow.MoveNext();
            }
            flexiCore.ReleaseAbility(ability);

            // Assert
            AbilityPool pool = flexiCore.GetAbilityPool(abilityHandle);
            Assert.AreEqual(0, pool.UsingCount);

            for (var i = 0; i < ability.Flows.Count; i++)
            {
                AbilityFlow flow = ability.Flows[i];
                Assert.AreEqual(null, flow.Current);
            }
        }

        [Test]
        public void AppendPoolizedAbilityToActor_UsingCountIs1()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            flexiCore.LoadAbility(abilityHandle, 1);

            _ = flexiCore.GetAbility(abilityHandle);
            Assert.AreEqual(1, flexiCore.GetAbilityPool(abilityHandle).UsingCount);
        }

        [Test]
        public void AppendPoolizedAbilityToActorAndRelease_UsingCountIs0()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            flexiCore.LoadAbility(abilityHandle, 1);

            Ability ability = flexiCore.GetAbility(abilityHandle);
            flexiCore.ReleaseAbility(ability);

            Assert.AreEqual(0, flexiCore.GetAbilityPool(abilityHandle).UsingCount);
        }

        [Test]
        public void ReleaseAbility_AbilityHasContainer_ContainerShouldBeNull()
        {
            AbilityHandle abilityHandle = AbilityTestHelper.CreateValidHandle();
            flexiCore.LoadAbility(abilityHandle, 1);

            var container = new DefaultAbilityContainer(abilityHandle.Data, abilityHandle.GroupIndex);
            Ability ability = flexiCore.GetAbility(abilityHandle);
            ability.Container = container;
            flexiCore.ReleaseAbility(ability);

            Assert.AreEqual(null, ability.Container);
        }
    }
}
