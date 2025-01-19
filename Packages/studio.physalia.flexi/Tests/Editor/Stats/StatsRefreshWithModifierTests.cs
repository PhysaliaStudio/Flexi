using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class StatsRefreshWithModifierTests
    {
        private AbilitySystemWrapperDefault wrapper;
        private AbilitySystem abilitySystem;

        [SetUp]
        public void SetUp()
        {
            wrapper = new AbilitySystemWrapperDefault();
            AbilitySystemBuilder builder = new AbilitySystemBuilder();
            builder.SetWrapper(wrapper);
            abilitySystem = builder.Build();
        }

        private EmptyActor CreateActor()
        {
            var actor = new EmptyActor();
            actor.AddStat(CustomStats.HEALTH, 100);
            actor.AddStat(CustomStats.MAX_HEALTH, 100);
            actor.AddStat(CustomStats.ATTACK, 12);
            wrapper.AppendActor(actor);
            return actor;
        }

        [Test]
        public void SetStat_OriginalBaseIs2AndNewBaseIs6_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            var actor = new EmptyActor();
            actor.AddStat(CustomStats.ATTACK, 2);
            wrapper.AppendActor(actor);

            actor.GetStat(CustomStats.ATTACK).CurrentBase = 6;
            wrapper.ApplyModifiers();

            Stat stat = actor.GetStat(CustomStats.ATTACK);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }

        [Test]
        public void ModifyStat_OriginalBaseIs2AndAdd4_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            var actor = new EmptyActor();
            actor.AddStat(CustomStats.ATTACK, 2);
            wrapper.AppendActor(actor);

            actor.GetStat(CustomStats.ATTACK).CurrentBase += 4;
            wrapper.ApplyModifiers();

            Stat stat = actor.GetStat(CustomStats.ATTACK);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }

        [Test]
        public void SingleModifier_WithStatIdOwnerNotOwned_NoError()
        {
            var actor = CreateActor();
            actor.RemoveStat(CustomStats.ATTACK);
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 50, StatModifier.Operator.MUL));
            wrapper.ApplyModifiers();

            Assert.IsNull(actor.GetStat(CustomStats.ATTACK));
            Assert.Pass();
        }

        [Test]
        public void SingleModifier_WithInvalidStatId_NoError()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(999, 50, StatModifier.Operator.MUL));
            wrapper.ApplyModifiers();

            Assert.Pass();
        }

        [Test]
        public void SingleModifier_100Minus10_CurrentBaseReturns100AndCurrentValueReturns90()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(CustomStats.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            wrapper.ApplyModifiers();

            Assert.AreEqual(100, actor.GetStat(CustomStats.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, actor.GetStat(CustomStats.MAX_HEALTH).CurrentValue);
        }

        [Test]
        public void SingleModifier_12Mul50Percent_CurrentBaseReturns12AndCurrentValueReturns18()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 50, StatModifier.Operator.MUL));
            wrapper.ApplyModifiers();

            Assert.AreEqual(12, actor.GetStat(CustomStats.ATTACK).CurrentBase);
            Assert.AreEqual(18, actor.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_Base12Add6AndMul50Percent_CurrentBaseReturns12AndCurrentValueReturns27()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 50, StatModifier.Operator.MUL));
            actor.AppendModifier(new StatModifier(CustomStats.ATTACK, 6, StatModifier.Operator.ADD));
            wrapper.ApplyModifiers();

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
            wrapper.ApplyModifiers();

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
            wrapper.ApplyModifiers();
            wrapper.ApplyModifiers();

            Assert.AreEqual(100, actor.GetStat(CustomStats.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, actor.GetStat(CustomStats.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, actor.GetStat(CustomStats.ATTACK).CurrentBase);
            Assert.AreEqual(27, actor.GetStat(CustomStats.ATTACK).CurrentValue);
        }
    }
}
