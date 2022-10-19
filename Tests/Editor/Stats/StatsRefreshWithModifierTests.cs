using NUnit.Framework;

namespace Physalia.AbilityFramework.Tests
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
            var modifier = new StatModifier();
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.MUL,
                value = 50,
            });

            var instance = new StatModifierInstance(modifier);
            var owner = CreateOwner();
            owner.RemoveStat(StatTestHelper.ATTACK);

            owner.AppendModifier(instance);
            repository.RefreshStats(owner);

            Assert.IsNull(owner.GetStat(StatTestHelper.ATTACK));
            Assert.Pass();
        }

        [Test]
        public void SingleModifier_WithInvalidStatId_NoError()
        {
            var modifier = new StatModifier();
            modifier.items.Add(new StatModifierItem
            {
                statId = 999,
                op = StatModifierItem.Operator.MUL,
                value = 50,
            });

            var instance = new StatModifierInstance(modifier);
            var owner = CreateOwner();

            owner.AppendModifier(instance);
            repository.RefreshStats(owner);

            Assert.Pass();
        }

        [Test]
        public void SingleModifier_100Minus10_CurrentBaseReturns100AndCurrentValueReturns90()
        {
            var modifier = new StatModifier();
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.MAX_HEALTH,
                op = StatModifierItem.Operator.ADD,
                value = -10,
            });

            var instance = new StatModifierInstance(modifier);
            var owner = CreateOwner();

            owner.AppendModifier(instance);
            repository.RefreshStats(owner);

            Assert.AreEqual(100, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
        }

        [Test]
        public void SingleModifier_12Mul50Percent_CurrentBaseReturns12AndCurrentValueReturns18()
        {
            var modifier = new StatModifier();
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.MUL,
                value = 50,
            });

            var instance = new StatModifierInstance(modifier);
            var owner = CreateOwner();

            owner.AppendModifier(instance);
            repository.RefreshStats(owner);

            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(18, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_Base12Add6AndMul50Percent_CurrentBaseReturns12AndCurrentValueReturns27()
        {
            var modifier = new StatModifier();
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.MUL,
                value = 50,
            });
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.ADD,
                value = 6,
            });

            var instance = new StatModifierInstance(modifier);
            var owner = CreateOwner();

            owner.AppendModifier(instance);
            repository.RefreshStats(owner);

            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void MultipleModifier_WithDifferentStatIds()
        {
            var modifier = new StatModifier();
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.MUL,
                value = 50,
            });
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.ADD,
                value = 6,
            });
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.MAX_HEALTH,
                op = StatModifierItem.Operator.ADD,
                value = -10,
            });

            var instance = new StatModifierInstance(modifier);
            var owner = CreateOwner();

            owner.AppendModifier(instance);
            repository.RefreshStats(owner);

            Assert.AreEqual(100, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }

        [Test]
        public void RefreshTwice_ReturnsTheSameValues()
        {
            var modifier = new StatModifier();
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.MUL,
                value = 50,
            });
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.ADD,
                value = 6,
            });
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.MAX_HEALTH,
                op = StatModifierItem.Operator.ADD,
                value = -10,
            });

            var instance = new StatModifierInstance(modifier);
            var owner = CreateOwner();

            owner.AppendModifier(instance);
            repository.RefreshStats(owner);
            repository.RefreshStats(owner);

            Assert.AreEqual(100, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentBase);
            Assert.AreEqual(90, owner.GetStat(StatTestHelper.MAX_HEALTH).CurrentValue);
            Assert.AreEqual(12, owner.GetStat(StatTestHelper.ATTACK).CurrentBase);
            Assert.AreEqual(27, owner.GetStat(StatTestHelper.ATTACK).CurrentValue);
        }
    }
}
