using Newtonsoft.Json;
using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class AbilityRunningTests
    {
        [Test]
        public void CanExecute_NotMatchCondition_ReturnsFalse()
        {
            var json = "{\"_type\":\"Physalia.Flexi.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.Flexi.StatRefreshEventNode\"}]," +
                "\"edges\":[]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            var abilityFlow = new AbilityFlow(abilityGraph);

            bool success = abilityFlow.CanExecute(null);
            Assert.AreEqual(false, success);
        }

        [Test]
        public void CanExecute_MatchCondition_ReturnsTrue()
        {
            var json = "{\"_type\":\"Physalia.Flexi.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.Flexi.StatRefreshEventNode\"}]," +
                "\"edges\":[]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            var abilityFlow = new AbilityFlow(abilityGraph);

            bool success = abilityFlow.CanExecute(new StatRefreshEvent());
            Assert.AreEqual(true, success);
        }
    }
}
