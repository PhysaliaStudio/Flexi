using NUnit.Framework;

namespace Physalia.Stats.Tests
{
    public class IntegrationTests
    {
        [Test]
        public void RefreshStats_WithSingleModifier()
        {
            var list = StatTestHelper.ValidList;
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(list);
            var ownerRepository = new StatOwnerRepository(table);
            StatOwner owner = ownerRepository.CreateOwner();

            owner.AddStat(11, 10);
            owner.AddModifier(new StatModifier { StatId = 11, Addend = 2, Multiplier = 50 });
            owner.RefreshStats();

            Assert.AreEqual(18, owner.GetStat(11).CurrentValue);
        }

        [Test]
        public void RefreshStats_WithMultipleModifiers()
        {
            var list = StatTestHelper.ValidList;
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(list);
            var ownerRepository = new StatOwnerRepository(table);
            StatOwner owner = ownerRepository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 10, Multiplier = 0 });
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
        }

        [Test]
        public void RefreshStats_WithModifiersContainsDifferentStatIds()
        {
            var list = StatTestHelper.ValidList;
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(list);
            var ownerRepository = new StatOwnerRepository(table);
            StatOwner owner = ownerRepository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddStat(11, 10);
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new StatModifier { StatId = 11, Addend = 2, Multiplier = 50 });
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 10, Multiplier = 0 });
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
            Assert.AreEqual(18, owner.GetStat(11).CurrentValue);
        }

        [Test]
        public void RefreshStats_WithModifiersContainsNotOwnedStatId()
        {
            var list = StatTestHelper.ValidList;
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(list);
            var ownerRepository = new StatOwnerRepository(table);
            StatOwner owner = ownerRepository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 10, Multiplier = 0 });
            owner.AddModifier(new StatModifier { StatId = 11, Addend = 10, Multiplier = 0 });
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
            Assert.IsNull(owner.GetStat(11));
        }

        [Test]
        public void RefreshStats_WithStatNotBeenModified()
        {
            var list = StatTestHelper.ValidList;
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(list);
            var ownerRepository = new StatOwnerRepository(table);
            StatOwner owner = ownerRepository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddStat(11, 10);
            owner.AddModifier(new Modifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new Modifier { StatId = 2, Addend = 10, Multiplier = 0 });
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
            Assert.AreEqual(10, owner.GetStat(11).CurrentValue);
        }

        [Test]
        public void RefreshStats_WithModifiersContainsInvalidStatId()
        {
            var list = StatTestHelper.ValidList;
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(list);
            var ownerRepository = new StatOwnerRepository(table);
            StatOwner owner = ownerRepository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 10, Multiplier = 0 });
            owner.AddModifier(new StatModifier { StatId = 999, Addend = 0, Multiplier = 0 });
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
            Assert.IsNull(owner.GetStat(11));
        }

        [Test]
        public void RefreshTwice_ReturnsTheSameValues()
        {
            var list = StatTestHelper.ValidList;
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(list);
            var ownerRepository = new StatOwnerRepository(table);
            StatOwner owner = ownerRepository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddStat(11, 10);
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new StatModifier { StatId = 11, Addend = 2, Multiplier = 50 });
            owner.AddModifier(new StatModifier { StatId = 2, Addend = 10, Multiplier = 0 });

            owner.RefreshStats();
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
            Assert.AreEqual(18, owner.GetStat(11).CurrentValue);
        }
    }
}
