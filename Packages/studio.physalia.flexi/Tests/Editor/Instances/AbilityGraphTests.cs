using Newtonsoft.Json;
using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class AbilityGraphTests
    {
        private const string TEST_JSON =
            "{\"_type\":\"Physalia.Flexi.AbilityGraph\"," +
            "\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.Flexi.StartNode\"}," +
            "{\"_id\":2,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.Flexi.Tests.CustomDamageNode\"}," +
            "{\"_id\":3,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.Flexi.Tests.GraphConverterTests+OwnerFilterNode\"}," +
            "{\"_id\":4,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.Flexi.IntegerNode\",\"value\":0}," +
            "{\"_id\":5,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.Flexi.LogNode\"}]," +
            "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
            "{\"id1\":2,\"port1\":\"next\",\"id2\":5,\"port2\":\"previous\"}," +
            "{\"id1\":3,\"port1\":\"owners\",\"id2\":2,\"port2\":\"targets\"}," +
            "{\"id1\":4,\"port1\":\"output\",\"id2\":2,\"port2\":\"baseValue\"}]}";

        [Test]
        public void CreateInstance_CurrentNodeIsNull()
        {
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(TEST_JSON);
            Assert.AreEqual(null, abilityGraph.Current);
        }

        [Test]
        public void MoveNext_AfterResetTo0_CurrentReturnsTheEntryNode()
        {
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(TEST_JSON);
            bool success = abilityGraph.MoveNext();
            Assert.AreEqual(true, success);
            Assert.AreEqual(typeof(StartNode), abilityGraph.Current.GetType());
        }

        [Test]
        public void ResetTo0_AfterMoveNext_CurrentNodeIsNull()
        {
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(TEST_JSON);
            bool success = abilityGraph.MoveNext();
            Assert.AreEqual(true, success);

            abilityGraph.Reset(0);
            Assert.AreEqual(null, abilityGraph.Current);
        }
    }
}
