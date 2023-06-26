using NUnit.Framework;
using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public class AbilityRunningTests
    {
        [HideFromSearchWindow]
        public class FlagEntryNode : EntryNode
        {
            public class Context : IEventContext
            {
                public int flag;
            }

            public Variable<int> startFlags;

            public override bool CanExecute(IEventContext context)
            {
                if (context is Context thisContext)
                {
                    if (startFlags == thisContext.flag)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
        }

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
        public void EmptyData_ReturnsFalse()
        {
            var abilityData = new AbilityData();
            abilityData.graphGroups.Add(new AbilityGraphGroup());
            Ability ability = abilitySystem.GetAbility(abilityData, 0);

            bool success = abilitySystem.TryEnqueueAbility(ability, null);
            Assert.AreEqual(false, success);
        }

        [Test]
        public void ContainsFlowWithoutNode_ReturnsFalse()
        {
            var json = "{\"_type\":\"Physalia.Flexi.AbilityGraph\",\"nodes\":[],\"edges\":[]}";
            var abilityData = AbilityTestHelper.CreateSingleGraphData(json);
            Ability ability = abilitySystem.GetAbility(abilityData, 0);

            bool success = abilitySystem.TryEnqueueAbility(ability, null);
            Assert.AreEqual(false, success);
            Assert.AreEqual(null, ability.Flows[0].Graph.EntryNodeAssigned);  // Since there is no entry node.
        }

        [Test]
        public void ContainsFlowWith1EntryNode_MatchTheContext_ReturnsTrueAndEntryNodeAssignedIsThe1stEntryNode()
        {
            var json = "{\"_type\":\"Physalia.Flexi.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.Flexi.Tests.AbilityRunningTests+FlagEntryNode\",startFlags:11}]," +
                "\"edges\":[]}";
            var abilityData = AbilityTestHelper.CreateSingleGraphData(json);
            Ability ability = abilitySystem.GetAbility(abilityData, 0);

            bool success = abilitySystem.TryEnqueueAbility(ability, new FlagEntryNode.Context { flag = 11 });
            Assert.AreEqual(true, success);
            Assert.AreEqual(ability.Flows[0].Graph.EntryNodes[0], ability.Flows[0].Graph.EntryNodeAssigned);
        }

        [Test]
        public void ContainsFlowWith2EntryNode_DoesNotMatchAnyContext_ReturnsFalse()
        {
            var json = "{\"_type\":\"Physalia.Flexi.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.Flexi.Tests.AbilityRunningTests+FlagEntryNode\",startFlags:11}," +
                "{\"_id\":2,\"_type\":\"Physalia.Flexi.Tests.AbilityRunningTests+FlagEntryNode\",startFlags:22}]," +
                "\"edges\":[]}";
            var abilityData = AbilityTestHelper.CreateSingleGraphData(json);
            Ability ability = abilitySystem.GetAbility(abilityData, 0);

            bool success = abilitySystem.TryEnqueueAbility(ability, new FlagEntryNode.Context { flag = 33 });
            Assert.AreEqual(false, success);

            // TODO: The default EntryNodeAssigned is the first entry node. This is wierd, though.
            // It's because I thought I shouldn't need to call Reset when I was implementing Graph.
            Assert.AreEqual(ability.Flows[0].Graph.EntryNodes[0], ability.Flows[0].Graph.EntryNodeAssigned);
        }

        [Test]
        public void ContainsFlowWith2EntryNode_MatchThe2ndContext_ReturnsTrueAndEntryNodeAssignedIsThe2ndEntryNode()
        {
            var json = "{\"_type\":\"Physalia.Flexi.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.Flexi.Tests.AbilityRunningTests+FlagEntryNode\",startFlags:11}," +
                "{\"_id\":2,\"_type\":\"Physalia.Flexi.Tests.AbilityRunningTests+FlagEntryNode\",startFlags:22}]," +
                "\"edges\":[]}";
            var abilityData = AbilityTestHelper.CreateSingleGraphData(json);
            Ability ability = abilitySystem.GetAbility(abilityData, 0);

            bool success = abilitySystem.TryEnqueueAbility(ability, new FlagEntryNode.Context { flag = 22 });
            Assert.AreEqual(true, success);
            Assert.AreEqual(ability.Flows[0].Graph.EntryNodes[1], ability.Flows[0].Graph.EntryNodeAssigned);
        }
    }
}
