using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.AbilitySystem.Tests
{
    public class IntegrationTests
    {
        [Test]
        public void CreateOwner_GetOwnerReturnsTheSameInstance()
        {
            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            var abilitySystem = new AbilitySystem(statDefinitionListAsset);
            StatOwner owner = abilitySystem.CreateOwner();
            Assert.AreEqual(owner, abilitySystem.GetOwner(owner.Id));
        }

        [Test]
        public void RemoveOwner_GetOwnerReturnsNull()
        {
            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            var abilitySystem = new AbilitySystem(statDefinitionListAsset);
            StatOwner owner = abilitySystem.CreateOwner();
            abilitySystem.RemoveOwner(owner);
            Assert.AreEqual(null, abilitySystem.GetOwner(owner.Id));
        }

        [Test]
        public void RunAbilityInstance_InstanceCanDoTheSameThingAsOriginal()
        {
            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            var abilitySystem = new AbilitySystem(statDefinitionListAsset);
            abilitySystem.LoadAbilityGraph(123456, CustomAbility.HELLO_WORLD);

            AbilityInstance instance = abilitySystem.GetAbilityInstance(123456);
            instance.Execute(null);

            // Check if the instance can do the same thing
            LogAssert.Expect(LogType.Log, "Hello");
            LogAssert.Expect(LogType.Log, "World!");
        }
    }
}
