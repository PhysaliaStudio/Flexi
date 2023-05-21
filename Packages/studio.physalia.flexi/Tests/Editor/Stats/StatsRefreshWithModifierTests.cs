using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class StatsRefreshWithModifierTests
    {
        private StatOwnerRepository repository;

        [SetUp]
        public void SetUp()
        {
            StatDefinitionListAsset statDefinitionList = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            repository = StatOwnerRepository.Create(statDefinitionList);
        }

        private StatOwner CreateOwner()
        {
            StatOwner owner = repository.CreateOwner();
            owner.AddStat(StatTestHelper.HEALTH, 100);
            owner.AddStat(StatTestHelper.MAX_HEALTH, 100);
            owner.AddStat(StatTestHelper.ATTACK, 12);
            return owner;
        }

        [Test]
        public void SingleModifier_WithStatIdOwnerNotOwned_NoError()
        {
            var owner = CreateOwner();
            owner.RemoveStat(StatTestHelper.ATTACK);
            owner.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            repository.RefreshStats(owner);

            Assert.IsNull(owner.GetStat(StatTestHelper.ATTACK));
            Assert.Pass();
        }

        [Test]
        public void SingleModifier_WithInvalidStatId_NoError()
        {
            var owner = CreateOwner();
            owner.AppendModifier(new StatModifier(999, 50, StatModifier.Operator.MUL));
            repository.RefreshStats(owner);

            Assert.Pass();
        }

        [Test]
        public void SingleModifier_100Minus10_CurrentBaseReturns100AndCurrentValueReturns90()
        {
            var owner = CreateOwner();
            owner.AppendModifier(new StatModifier(StatTestHelper.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            repository.RefreshStats(owner);

            Assert.AreEqual(100, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
        }

        [Test]
        public void SingleModifier_12Mul50Percent_CurrentBaseReturns12AndCurrentValueReturns18()
        {
            var owner = CreateOwner();
            owner.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            repository.RefreshStats(owner);

            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(18, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_Base12Add6AndMul50Percent_CurrentBaseReturns12AndCurrentValueReturns27()
        {
            var owner = CreateOwner();
            owner.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            owner.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 6, StatModifier.Operator.ADD));
            repository.RefreshStats(owner);

            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_WithDifferentStatIds()
        {
            var owner = CreateOwner();
            owner.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            owner.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 6, StatModifier.Operator.ADD));
            owner.AppendModifier(new StatModifier(StatTestHelper.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            repository.RefreshStats(owner);

            Assert.AreEqual(100, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void RefreshTwice_ReturnsTheSameValues()
        {
            var owner = CreateOwner();
            owner.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 50, StatModifier.Operator.MUL));
            owner.AppendModifier(new StatModifier(StatTestHelper.ATTACK, 6, StatModifier.Operator.ADD));
            owner.AppendModifier(new StatModifier(StatTestHelper.MAX_HEALTH, -10, StatModifier.Operator.ADD));
            repository.RefreshStats(owner);
            repository.RefreshStats(owner);

            Assert.AreEqual(100, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }
    }
}
