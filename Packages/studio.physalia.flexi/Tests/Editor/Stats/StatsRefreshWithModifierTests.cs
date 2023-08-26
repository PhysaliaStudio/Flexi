using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class StatsRefreshWithModifierTests
    {
        private AbilitySystem abilitySystem;

        [SetUp]
        public void SetUp()
        {
            AbilitySystemBuilder builder = new AbilitySystemBuilder();
            abilitySystem = builder.Build();
        }

        private Actor CreateActor()
        {
            var actor = new EmptyActor(abilitySystem);
            actor.AddStat(CustomStats.HEALTH, 100);
            actor.AddStat(CustomStats.MAX_HEALTH, 100);
            actor.AddStat(CustomStats.ATTACK, 12);
            return actor;
        }

        [Test]
        public void SingleModifier_WithStatIdOwnerNotOwned_NoError()
        {
            var actor = CreateActor();
            actor.RemoveStat(CustomStats.ATTACK);
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 50, StatModifier.Operator.MUL));
            actor.RefreshStats();

            Assert.IsNull(actor.GetStat(CustomStats.ATTACK));
            Assert.Pass();
        }

        [Test]
        public void SingleModifier_WithInvalidStatId_NoError()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(999, 50, StatModifier.Operator.MUL));
            actor.RefreshStats();

            Assert.Pass();
        }

        [Test]
        public void SingleModifier_100Minus10_CurrentBaseReturns100AndCurrentValueReturns90()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(CustomStats.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            actor.RefreshStats();

            Assert.AreEqual(100, actor.GetStat(CustomStats.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, actor.GetStat(CustomStats.MAX_HEALTH).CurrentValue);
        }

        [Test]
        public void SingleModifier_12Mul50Percent_CurrentBaseReturns12AndCurrentValueReturns18()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 50, StatModifier.Operator.MUL));
            actor.RefreshStats();

            Assert.AreEqual(12, actor.GetStat(CustomStats.ATTACK).CurrentBase);
            Assert.AreEqual(18, actor.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_Base12Add6AndMul50Percent_CurrentBaseReturns12AndCurrentValueReturns27()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 50, StatModifier.Operator.MUL));
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 6, StatModifier.Operator.ADD));
            actor.RefreshStats();

            Assert.AreEqual(12, actor.GetStat(CustomStats.ATTACK).CurrentBase);
            Assert.AreEqual(27, actor.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_WithDifferentStatIds()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 50, StatModifier.Operator.MUL));
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 6, StatModifier.Operator.ADD));
            actor.AppendModifier(new StatModifier(CustomStats.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            actor.RefreshStats();

            Assert.AreEqual(100, actor.GetStat(CustomStats.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, actor.GetStat(CustomStats.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, actor.GetStat(CustomStats.ATTACK).CurrentBase);
            Assert.AreEqual(27, actor.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void RefreshTwice_ReturnsTheSameValues()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 50, StatModifier.Operator.MUL));
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 6, StatModifier.Operator.ADD));
            actor.AppendModifier(new StatModifier(CustomStats.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            actor.RefreshStats();
            actor.RefreshStats();

            Assert.AreEqual(100, actor.GetStat(CustomStats.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, actor.GetStat(CustomStats.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, actor.GetStat(CustomStats.ATTACK).CurrentBase);
            Assert.AreEqual(27, actor.GetStat(CustomStats.ATTACK).CurrentValue);
        }
    }
}
