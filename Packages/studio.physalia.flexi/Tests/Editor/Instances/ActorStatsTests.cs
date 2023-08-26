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
            abilitySystem = builder.Build();
        }

        [Test]
        public void SetStat_OriginalBaseIs2AndNewBaseIs6_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            Actor actor = new EmptyActor(abilitySystem);

            actor.AddStat(CustomStats.ATTACK, 2);
            actor.SetStat(CustomStats.ATTACK, 6);

            Stat stat = actor.GetStat(CustomStats.ATTACK);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }

        [Test]
        public void ModifyStat_OriginalBaseIs2AndAdd4_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            Actor actor = new EmptyActor(abilitySystem);

            actor.AddStat(CustomStats.ATTACK, 2);
            actor.ModifyStat(CustomStats.ATTACK, 4);

            Stat stat = actor.GetStat(CustomStats.ATTACK);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }
    }
}
