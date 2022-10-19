using Newtonsoft.Json;
using NUnit.Framework;

namespace Physalia.AbilityFramework.Tests
{
    public class AbilityGraphTests
    {
        private const string TEST_JSON =
            "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
            "\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
            "{\"_id\":2,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.Tests.GraphConverterTests+DamageNode\"}," +
            "{\"_id\":3,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.Tests.GraphConverterTests+OwnerFilterNode\"}," +
            "{\"_id\":4,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.Tests.GraphConverterTests+IntNode\",\"value\":0}," +
            "{\"_id\":5,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilityFramework.Tests.GraphConverterTests+LogNode\"}]," +
            "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
            "{\"id1\":2,\"port1\":\"next\",\"id2\":5,\"port2\":\"previous\"}," +
            "{\"id1\":2,\"port1\":\"owners\",\"id2\":3,\"port2\":\"owners\"}," +
            "{\"id1\":2,\"port1\":\"baseValue\",\"id2\":4,\"port2\":\"output\"}]}";

        [Test]
        public void CreateInstance_CurrentNodeIsNull()
        {
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(TEST_JSON);
            Assert.AreEqual(null, abilityGraph.Current);
        }

        [Test]
        public void ResetTo0_CurrentNodeIsTheIndex0OfEntryNodes()
        {
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(TEST_JSON);
            abilityGraph.Reset(0);
            Assert.AreEqual(abilityGraph.Nodes[0], abilityGraph.Current);
        }

        [Test]
        public void MoveNext_AfterResetTo0_CurrentReturnsCorrectType()
        {
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(TEST_JSON);
            abilityGraph.Reset(0);

            bool success = abilityGraph.MoveNext();
            Assert.AreEqual(true, success);
            Assert.AreEqual(typeof(GraphConverterTests.DamageNode), abilityGraph.Current.GetType());
        }
    }
}
