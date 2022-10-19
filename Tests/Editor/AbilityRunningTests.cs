using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.AbilityFramework.Tests
{
    public class AbilityRunningTests
    {
        [Test]
        public void RunAbility_ToFinish_DoAllTasksAndCurrentStateReturnsDone()
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
            instance.Execute();

            LogAssert.Expect(LogType.Log, "Hello");
            LogAssert.Expect(LogType.Log, "World!");

            Assert.AreEqual(AbilityState.DONE, instance.CurrentState);
        }

        [Test]
        public void RunAbility_WithCustomPayload_DoTasksWithPayload()
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
            var unit = new CustomUnit(new CustomUnitData { name = "Mob1", }, owner);

            var payload = new CustomPayload { owner = unit, };
            instance.SetPayload(payload);
            instance.Execute();

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

            instance.Execute();

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

            instance.Execute();
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
            instance.Execute();

            instance.Resume();

            LogAssert.Expect(LogType.Error, new Regex(".*"));
        }

        [Test]
        public void RunAbility_ExecutePausedAbility_LogError()
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

            instance.Execute();  // This will encounter pause
            instance.Execute();  // Then execute again

            LogAssert.Expect(LogType.Log, "Ready to Pause!");
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RunAbility_ResetPausedAbility_CurrentStateReturnsCleanAndCanExecuteAgain()
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

            instance.Execute();  // This will encounter pause
            instance.Reset();

            Assert.AreEqual(AbilityState.CLEAN, instance.CurrentState);

            instance.Execute();  // Then execute again

            LogAssert.Expect(LogType.Log, "Ready to Pause!");
            LogAssert.Expect(LogType.Log, "Ready to Pause!");
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void ConditionalTrigger_NoCondition()
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

            Assert.AreEqual(true, instance.CanExecute(null));
        }

        [Test]
        public void ConditionalTrigger_FailedWithEmptyCondition()
        {
            var json = "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilitySystem.Tests.CustomDamageEventNode\"}," +
                "{\"_id\":747695,\"_position\":{\"x\":240,\"y\":161},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":524447,\"_position\":{\"x\":468,\"y\":175},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":135698,\"_position\":{\"x\":72,\"y\":292},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"I'm damaged!\"}," +
                "{\"_id\":675591,\"_position\":{\"x\":270,\"y\":316},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"I will revenge!\"}]," +
                "\"edges\":[{\"id1\":507088,\"port1\":\"next\",\"id2\":747695,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"next\",\"id2\":524447,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"text\",\"id2\":135698,\"port2\":\"output\"}," +
                "{\"id1\":524447,\"port1\":\"text\",\"id2\":675591,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            Assert.AreEqual(false, instance.CanExecute(null));

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void ConditionalTrigger_SuccessWhenMatchCondition()
        {
            var json = "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilitySystem.Tests.CustomDamageEventNode\"}," +
                "{\"_id\":747695,\"_position\":{\"x\":240,\"y\":161},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":524447,\"_position\":{\"x\":468,\"y\":175},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":135698,\"_position\":{\"x\":72,\"y\":292},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"I'm damaged!\"}," +
                "{\"_id\":675591,\"_position\":{\"x\":270,\"y\":316},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"I will revenge!\"}]," +
                "\"edges\":[{\"id1\":507088,\"port1\":\"next\",\"id2\":747695,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"next\",\"id2\":524447,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"text\",\"id2\":135698,\"port2\":\"output\"}," +
                "{\"id1\":524447,\"port1\":\"text\",\"id2\":675591,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            StatOwnerRepository ownerRepository = StatOwnerRepository.Create(statDefinitionListAsset);
            StatOwner owner = ownerRepository.CreateOwner();
            var unit = new CustomUnit(new CustomUnitData { name = "Mob1", }, owner);

            instance.SetOwner(owner);
            var payload = new CustomDamageEvent { target = unit, };

            Assert.AreEqual(true, instance.CanExecute(payload));

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RunAbility_WithConditionFailed_DoNotExecute()
        {
            var json = "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilitySystem.Tests.CustomDamageEventNode\"}," +
                "{\"_id\":747695,\"_position\":{\"x\":240,\"y\":161},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":524447,\"_position\":{\"x\":468,\"y\":175},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":135698,\"_position\":{\"x\":72,\"y\":292},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"I'm damaged!\"}," +
                "{\"_id\":675591,\"_position\":{\"x\":270,\"y\":316},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"I will revenge!\"}]," +
                "\"edges\":[{\"id1\":507088,\"port1\":\"next\",\"id2\":747695,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"next\",\"id2\":524447,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"text\",\"id2\":135698,\"port2\":\"output\"}," +
                "{\"id1\":524447,\"port1\":\"text\",\"id2\":675591,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            instance.Execute();

            LogAssert.Expect(LogType.Error, new Regex(".*"));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void RunAbility_WithConditionSuccess_ExecuteAsExpected()
        {
            var json = "{\"_type\":\"Physalia.AbilitySystem.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilitySystem.Tests.CustomDamageEventNode\"}," +
                "{\"_id\":747695,\"_position\":{\"x\":240,\"y\":161},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":524447,\"_position\":{\"x\":468,\"y\":175},\"_type\":\"Physalia.AbilitySystem.LogNode\"}," +
                "{\"_id\":135698,\"_position\":{\"x\":72,\"y\":292},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"I'm damaged!\"}," +
                "{\"_id\":675591,\"_position\":{\"x\":270,\"y\":316},\"_type\":\"Physalia.AbilitySystem.StringNode\",\"text\":\"I will revenge!\"}]," +
                "\"edges\":[{\"id1\":507088,\"port1\":\"next\",\"id2\":747695,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"next\",\"id2\":524447,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"text\",\"id2\":135698,\"port2\":\"output\"}," +
                "{\"id1\":524447,\"port1\":\"text\",\"id2\":675591,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            StatOwnerRepository ownerRepository = StatOwnerRepository.Create(statDefinitionListAsset);
            StatOwner owner = ownerRepository.CreateOwner();
            var unit = new CustomUnit(new CustomUnitData { name = "Mob1", }, owner);

            instance.SetOwner(owner);
            var payload = new CustomDamageEvent { target = unit, };

            instance.SetPayload(payload);
            instance.Execute();

            LogAssert.Expect(LogType.Log, "I'm damaged!");
            LogAssert.Expect(LogType.Log, "I will revenge!");
            LogAssert.NoUnexpectedReceived();
        }
    }
}
