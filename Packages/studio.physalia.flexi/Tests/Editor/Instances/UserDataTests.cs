using NUnit.Framework;
using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public class UserDataTests
    {
        private class TestUserData
        {
            public string text;
        }

        private class TestUserDataAnother
        {
            public int p;
        }

        private AbilitySystem abilitySystem;

        [SetUp]
        public void SetUp()
        {
            AbilitySystemBuilder builder = new AbilitySystemBuilder();

            var statDefinitionListAsset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
            statDefinitionListAsset.stats.AddRange(CustomStats.List);
            builder.SetStatDefinitions(statDefinitionListAsset);

            abilitySystem = builder.Build();
        }

        [Test]
        public void SetUserDataWithInstantiation_GetUserDataReturnsTheSameObject()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            var userData = new TestUserData();
            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource, userData);

            Assert.AreEqual(userData, ability.GetUserData<TestUserData>());
        }

        [Test]
        public void SetUserDataWithMethod_GetUserDataReturnsTheSameObject()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            var userData = new TestUserData();
            ability.SetUserData(userData);

            Assert.AreEqual(userData, ability.GetUserData<TestUserData>());
        }

        [Test]
        public void UserDataIsNull_GetUserDataLogsWarning()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource);
            var userData = ability.GetUserData<TestUserData>();

            Assert.IsNull(userData);
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }

        [Test]
        public void UserDataIsNotNull_GetUserDataWithAnotherTypeLogsWarning()
        {
            AbilityDataSource abilityDataSource = AbilityTestHelper.CreateValidDataSource();
            Ability ability = abilitySystem.InstantiateAbility(abilityDataSource, new TestUserData());
            var userData = ability.GetUserData<TestUserDataAnother>();

            Assert.IsNull(userData);
            TestUtilities.LogAssertAnyString(LogType.Warning);
        }
    }
}
