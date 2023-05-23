using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class ActorStatsTests
    {
        private AbilitySystem abilitySystem;

        [SetUp]
        public void SetUp()
        {
            AbilitySystemBuilder builder = new AbilitySystemBuilder();

            StatDefinitionListAsset statDefinitionListAsset = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            builder.SetStatDefinitions(statDefinitionListAsset);

            abilitySystem = builder.Build();
        }

        [Test]
        public void SetStat_OriginalBaseIs2AndNewBaseIs6_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            Actor actor = new EmptyActor(abilitySystem);

            actor.AddStat(StatTestHelper.ATTACK, 2);
            actor.SetStat(StatTestHelper.ATTACK, 6);

            Stat stat = actor.GetStat(StatTestHelper.ATTACK);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }

        [Test]
        public void ModifyStat_OriginalBaseIs2AndAdd4_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            Actor actor = new EmptyActor(abilitySystem);

            actor.AddStat(StatTestHelper.ATTACK, 2);
            actor.ModifyStat(StatTestHelper.ATTACK, 4);

            Stat stat = actor.GetStat(StatTestHelper.ATTACK);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }
    }
}
