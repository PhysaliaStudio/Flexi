using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.AbilitySystem.Tests
{
    public class AbilityRunningTests
    {
        [Test]
        public void RunAbility_WithoutOwner()
        {
            var json = "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilitySystem.StartNode\"}," +
                "{\"_id\":747695,\"_position\":{\"x\":240,\"y\":161},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":524447,\"_position\":{\"x\":468,\"y\":175},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":675591,\"_position\":{\"x\":270,\"y\":316},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"World!\"}," +
                "{\"_id\":135698,\"_position\":{\"x\":72,\"y\":292},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"Hello\"}]," +
                "\"edges\":[{\"id1\":507088,\"port1\":\"next\",\"id2\":747695,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"next\",\"id2\":524447,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"text\",\"id2\":135698,\"port2\":\"output\"}," +
                "{\"id1\":524447,\"port1\":\"text\",\"id2\":675591,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);
            instance.Execute(null);

            LogAssert.Expect(LogType.Log, "Hello");
            LogAssert.Expect(LogType.Log, "World!");
        }

        [Test]
        public void RunAbility_WithOwner()
        {
            var json = "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":0,\"_type\":\"Physalia.AbilitySystem.StartNode\"}," +
                "{\"_id\":1,\"_type\":\"Physalia.AbilitySystem.Tests.CustomPayloadNode\"}," +
                "{\"_id\":2,\"_type\":\"Physalia.AbilitySystem.Tests.LogCharacterNameNode\"}]," +
                "\"edges\":[{\"id1\":0,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
                "{\"id1\":2,\"port1\":\"character\",\"id2\":1,\"port2\":\"owner\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            StatOwnerRepository ownerRepository = StatOwnerRepository.Create(statDefinitionListAsset);
            StatOwner owner = ownerRepository.CreateOwner();
            var character = new Character
            {
                name = "Mob1",
                statOwner = owner,
            };

            var payload = new CustomPayload { owner = character, };
            instance.Execute(payload);

            LogAssert.Expect(LogType.Log, "My name is Mob1");
        }

        [Test]
        public void RunAbility_EncounterPauseState()
        {
            var json = "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.AbilitySystem.StartNode\"}," +
                "{\"_id\":2,\"_type\":\"Physalia.AbilitySystem.Tests.PauseNode\"}," +
                "{\"_id\":3,\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"Ready to Pause!\"}," +
                "{\"_id\":4,\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":5,\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":6,\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"Hello World!\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":4,\"port2\":\"previous\"}," +
                "{\"id1\":4,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
                "{\"id1\":4,\"port1\":\"text\",\"id2\":3,\"port2\":\"output\"}," +
                "{\"id1\":2,\"port1\":\"next\",\"id2\":5,\"port2\":\"previous\"}," +
                "{\"id1\":5,\"port1\":\"text\",\"id2\":6,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            instance.Execute(null);

            LogAssert.Expect(LogType.Log, "Ready to Pause!");
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RunAbility_ResumeFromPauseState()
        {
            var json = "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.AbilitySystem.StartNode\"}," +
                "{\"_id\":2,\"_type\":\"Physalia.AbilitySystem.Tests.PauseNode\"}," +
                "{\"_id\":3,\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"Ready to Pause!\"}," +
                "{\"_id\":4,\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":5,\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":6,\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"Hello World!\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":4,\"port2\":\"previous\"}," +
                "{\"id1\":4,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
                "{\"id1\":4,\"port1\":\"text\",\"id2\":3,\"port2\":\"output\"}," +
                "{\"id1\":2,\"port1\":\"next\",\"id2\":5,\"port2\":\"previous\"}," +
                "{\"id1\":5,\"port1\":\"text\",\"id2\":6,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            instance.Execute(null);
            instance.Resume();

            LogAssert.Expect(LogType.Log, "Ready to Pause!");
            LogAssert.Expect(LogType.Log, "Hello World!");
        }

        [Test]
        public void RunAbility_ResumeAlreadyFinishedAbility_LogError()
        {
            var json = "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_type\":\"Physalia.AbilitySystem.StartNode\"}," +
                "{\"_id\":747695,\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":524447,\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":675591,\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"World!\"}," +
                "{\"_id\":135698,\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"Hello\"}]," +
                "\"edges\":[{\"id1\":507088,\"port1\":\"next\",\"id2\":747695,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"next\",\"id2\":524447,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"text\",\"id2\":135698,\"port2\":\"output\"}," +
                "{\"id1\":524447,\"port1\":\"text\",\"id2\":675591,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);
            instance.Execute(null);

            instance.Resume();

            LogAssert.Expect(LogType.Error, new Regex(".*"));
        }
    }
}
