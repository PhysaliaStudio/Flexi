using Newtonsoft.Json;
using NUnit.Framework;

namespace Physalia.AbilitySystem.Tests
{
    public class AbilityInstanceTests
    {
        private const string TEST_JSON =
            "{\"_type\":\"Physalia.AbilitySystem.Ability\"," +
            "\"nodes\":[{\"_id\":1,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.StartNode\"}," +
            "{\"_id\":2,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+DamageNode\"}," +
            "{\"_id\":3,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+OwnerFilterNode\"}," +
            "{\"_id\":4,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+IntNode\",\"value\":0}," +
            "{\"_id\":5,\"_position\":{\"x\":0.0,\"y\":0.0},\"_type\":\"Physalia.AbilitySystem.Tests.GraphConverterTests+LogNode\"}]," +
            "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
            "{\"id1\":2,\"port1\":\"next\",\"id2\":5,\"port2\":\"previous\"}," +
            "{\"id1\":2,\"port1\":\"owners\",\"id2\":3,\"port2\":\"owners\"}," +
            "{\"id1\":2,\"port1\":\"baseValue\",\"id2\":4,\"port2\":\"output\"}]}";

        [Test]
        public void CreateInstance_CurrentNodeIsTheIndex0OfEntryNodes()
        {
            Ability ability = JsonConvert.DeserializeObject<Ability>(TEST_JSON);
            AbilityInstance instance = ability.CreateInstance();
            Assert.AreEqual(ability.Nodes[0], instance.Current.Node);
        }

        [Test]
        public void ResetTo0_CurrentNodeIsTheIndex0OfEntryNodes()
        {
            Ability ability = JsonConvert.DeserializeObject<Ability>(TEST_JSON);
            AbilityInstance instance = ability.CreateInstance();
            Assert.AreEqual(ability.Nodes[0], instance.Current.Node);
        }

        [Test]
        public void MoveNext_CurrentBecomesNext()
        {
            Ability ability = JsonConvert.DeserializeObject<Ability>(TEST_JSON);
            AbilityInstance instance = ability.CreateInstance();

            bool success = instance.MoveNext();
            Assert.AreEqual(true, success);
            Assert.AreEqual(typeof(GraphConverterTests.DamageNodeLogic), instance.Current.GetType());
            Assert.AreEqual(typeof(GraphConverterTests.DamageNode), instance.Current.Node.GetType());
        }

        [Test]
        public void GetNodeLogic_ReturnsWithCorrectType()
        {
            Ability ability = JsonConvert.DeserializeObject<Ability>(TEST_JSON);
            AbilityInstance instance = ability.CreateInstance();
            _ = instance.MoveNext();

            NodeLogic nodeLogic = instance.Current;
            Assert.AreEqual(typeof(GraphConverterTests.DamageNodeLogic), nodeLogic.GetType());
        }
    }
}
