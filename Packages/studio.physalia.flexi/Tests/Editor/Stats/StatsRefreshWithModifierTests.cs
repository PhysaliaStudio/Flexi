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

            StatDefinitionListAsset statDefinitionListAsset = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            builder.SetStatDefinitions(statDefinitionListAsset);

            abilitySystem = builder.Build();
        }

        private Actor CreateActor()
        {
            var actor = new EmptyActor(abilitySystem);
            actor.AddStat(StatTestHelper.HEALTH, 100);
            actor.AddStat(StatTestHelper.MAX_HEALTH, 100);
            actor.AddStat(StatTestHelper.ATTACK, 12);
            return actor;
        }

        [Test]
        public void SingleModifier_WithStatIdOwnerNotOwned_NoError()
        {
            var actor = CreateActor();
            actor.RemoveStat(StatTestHelper.ATTACK);
            actor.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            actor.RefreshStats();

            Assert.IsNull(actor.GetStat(StatTestHelper.ATTACK));
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
            actor.AppendModifier(new StatModifier(StatTestHelper.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            actor.RefreshStats();

            Assert.AreEqual(100, actor.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, actor.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
        }

        [Test]
        public void SingleModifier_12Mul50Percent_CurrentBaseReturns12AndCurrentValueReturns18()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            actor.RefreshStats();

            Assert.AreEqual(12, actor.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(18, actor.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_Base12Add6AndMul50Percent_CurrentBaseReturns12AndCurrentValueReturns27()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            actor.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 6, StatModifier.Operator.ADD));
            actor.RefreshStats();

            Assert.AreEqual(12, actor.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, actor.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_WithDifferentStatIds()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            actor.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 6, StatModifier.Operator.ADD));
            actor.AppendModifier(new StatModifier(StatTestHelper.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            actor.RefreshStats();

            Assert.AreEqual(100, actor.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, actor.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, actor.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, actor.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void RefreshTwice_ReturnsTheSameValues()
        {
            var actor = CreateActor();
            actor.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            actor.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 6, StatModifier.Operator.ADD));
            actor.AppendModifier(new StatModifier(StatTestHelper.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            actor.RefreshStats();
            actor.RefreshStats();

            Assert.AreEqual(100, actor.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, actor.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, actor.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, actor.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }
    }
}
