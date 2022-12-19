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
        public void GetBlackboardVariable_DataIs5_Returns5()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"variables\":[{\"key\":\"damage\",\"value\":5}]," +
                "\"nodes\":[],\"edges\":[]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            Assert.AreEqual(5, instance.GetBlackboardVariable("damage"));
        }

        [Test]
        public void GetBlackboardVariable_NoData_Returns0()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"variables\":[{\"key\":\"damage\",\"value\":5}]," +
                "\"nodes\":[],\"edges\":[]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            Assert.AreEqual(0, instance.GetBlackboardVariable("not-exist"));
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void OverrideBlackboardVariable_NoData_LogWarningAndGetReturns0()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"variables\":[{\"key\":\"damage\",\"value\":5}]," +
                "\"nodes\":[],\"edges\":[]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);
            instance.OverrideBlackboardVariable("not-exist", 10);

            TestUtilities.LogAssertAnyString(LogType.Warning);

            Assert.AreEqual(0, instance.GetBlackboardVariable("not-exist"));
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void OverrideBlackboardVariable_DataIs5AndOverrideTo10_GetReturns10()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"variables\":[{\"key\":\"damage\",\"value\":5}]," +
                "\"nodes\":[],\"edges\":[]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);
            instance.OverrideBlackboardVariable("damage", 10);

            Assert.AreEqual(10, instance.GetBlackboardVariable("damage"));
        }

        [Test]
        public void OverrideBlackboardVariable_DataIs5AndOverrideTo10ThenReset_GetReturns5()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"variables\":[{\"key\":\"damage\",\"value\":5}]," +
                "\"nodes\":[],\"edges\":[]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);
            instance.OverrideBlackboardVariable("damage", 10);
            instance.Reset();

            Assert.AreEqual(5, instance.GetBlackboardVariable("damage"));
        }

        [Test]
        public void RunAbility_ToFinish_DoAllTasksAndCurrentStateReturnsDone()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
                "{\"_id\":747695,\"_position\":{\"x\":240,\"y\":161},\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":524447,\"_position\":{\"x\":468,\"y\":175},\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":675591,\"_position\":{\"x\":270,\"y\":316},\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"World!\"}," +
                "{\"_id\":135698,\"_position\":{\"x\":72,\"y\":292},\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Hello\"}]," +
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
        public void RunAbility_EncounterPauseState()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
                "{\"_id\":2,\"_type\":\"Physalia.AbilityFramework.Tests.PauseNode\"}," +
                "{\"_id\":3,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Ready to Pause!\"}," +
                "{\"_id\":4,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":5,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":6,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Hello World!\"}]," +
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
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
                "{\"_id\":2,\"_type\":\"Physalia.AbilityFramework.Tests.PauseNode\"}," +
                "{\"_id\":3,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Ready to Pause!\"}," +
                "{\"_id\":4,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":5,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":6,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Hello World!\"}]," +
                "\"edges\":[{\"id1\":1,\"port1\":\"next\",\"id2\":4,\"port2\":\"previous\"}," +
                "{\"id1\":4,\"port1\":\"next\",\"id2\":2,\"port2\":\"previous\"}," +
                "{\"id1\":4,\"port1\":\"text\",\"id2\":3,\"port2\":\"output\"}," +
                "{\"id1\":2,\"port1\":\"next\",\"id2\":5,\"port2\":\"previous\"}," +
                "{\"id1\":5,\"port1\":\"text\",\"id2\":6,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            instance.Execute();
            instance.Resume(null);

            LogAssert.Expect(LogType.Log, "Ready to Pause!");
            LogAssert.Expect(LogType.Log, "Hello World!");
        }

        [Test]
        public void RunAbility_ResumeAlreadyFinishedAbility_LogError()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
                "{\"_id\":747695,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":524447,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":675591,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"World!\"}," +
                "{\"_id\":135698,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Hello\"}]," +
                "\"edges\":[{\"id1\":507088,\"port1\":\"next\",\"id2\":747695,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"next\",\"id2\":524447,\"port2\":\"previous\"}," +
                "{\"id1\":747695,\"port1\":\"text\",\"id2\":135698,\"port2\":\"output\"}," +
                "{\"id1\":524447,\"port1\":\"text\",\"id2\":675591,\"port2\":\"output\"}]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);
            instance.Execute();

            instance.Resume(null);

            LogAssert.Expect(LogType.Error, new Regex(".*"));
        }

        [Test]
        public void RunAbility_ExecutePausedAbility_LogError()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
                "{\"_id\":2,\"_type\":\"Physalia.AbilityFramework.Tests.PauseNode\"}," +
                "{\"_id\":3,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Ready to Pause!\"}," +
                "{\"_id\":4,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":5,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":6,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Hello World!\"}]," +
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
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":1,\"_type\":\"Physalia.AbilityFramework.StartNode\"}," +
                "{\"_id\":2,\"_type\":\"Physalia.AbilityFramework.Tests.PauseNode\"}," +
                "{\"_id\":3,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Ready to Pause!\"}," +
                "{\"_id\":4,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":5,\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":6,\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"Hello World!\"}]," +
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
        public void CanExecute_NotMatchCondition_ReturnsFalse()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilityFramework.StatRefreshEventNode\"}]," +
                "\"edges\":[]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            bool success = instance.CanExecute(null);
            Assert.AreEqual(false, success);
        }

        [Test]
        public void CanExecute_MatchCondition_ReturnsTrue()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilityFramework.StatRefreshEventNode\"}]," +
                "\"edges\":[]}";
            AbilityGraph abilityGraph = JsonConvert.DeserializeObject<AbilityGraph>(json);
            AbilityInstance instance = new AbilityInstance(abilityGraph);

            bool success = instance.CanExecute(new StatRefreshEvent());
            Assert.AreEqual(true, success);
        }

        [Test]
        public void RunAbility_WithConditionFailed_DoNotExecute()
        {
            var json = "{\"_type\":\"Physalia.AbilityFramework.AbilityGraph\"," +
                "\"nodes\":[{\"_id\":507088,\"_position\":{\"x\":54,\"y\":98},\"_type\":\"Physalia.AbilityFramework.Tests.CustomDamageEventNode\"}," +
                "{\"_id\":747695,\"_position\":{\"x\":240,\"y\":161},\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":524447,\"_position\":{\"x\":468,\"y\":175},\"_type\":\"Physalia.AbilityFramework.LogNode\"}," +
                "{\"_id\":135698,\"_position\":{\"x\":72,\"y\":292},\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"I'm damaged!\"}," +
                "{\"_id\":675591,\"_position\":{\"x\":270,\"y\":316},\"_type\":\"Physalia.AbilityFramework.StringNode\",\"text\":\"I will revenge!\"}]," +
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
    }
}
