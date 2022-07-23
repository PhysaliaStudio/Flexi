using NUnit.Framework;

namespace Physalia.AbilitySystem.StatSystem.Tests
{
    public class IntegrationTests
    {
        [Test]
        public void RefreshStats_WithSingleModifier()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 10);
            owner.AddModifier(new Modifier { StatId = 11, Addend = 2, Multiplier = 50 });
            owner.RefreshStats();

            Assert.AreEqual(18, owner.GetStat(11).CurrentValue);
        }

        [Test]
        public void RefreshStats_WithMultipleModifiers()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddModifier(new Modifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new Modifier { StatId = 2, Addend = 10, Multiplier = 0 });
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
        }

        [Test]
        public void RefreshStats_WithModifiersContainsDifferentStatIds()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddStat(11, 10);
            owner.AddModifier(new Modifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new Modifier { StatId = 11, Addend = 2, Multiplier = 50 });
            owner.AddModifier(new Modifier { StatId = 2, Addend = 10, Multiplier = 0 });
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
            Assert.AreEqual(18, owner.GetStat(11).CurrentValue);
        }

        [Test]
        public void RefreshStats_WithModifiersContainsNotOwnedStatId()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddModifier(new Modifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new Modifier { StatId = 2, Addend = 10, Multiplier = 0 });
            owner.AddModifier(new Modifier { StatId = 11, Addend = 10, Multiplier = 0 });
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
            Assert.IsNull(owner.GetStat(11));
        }

        [Test]
        public void RefreshStats_WithStatNotBeenModified()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

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
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddModifier(new Modifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new Modifier { StatId = 2, Addend = 10, Multiplier = 0 });
            owner.AddModifier(new Modifier { StatId = 999, Addend = 0, Multiplier = 0 });
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
            Assert.IsNull(owner.GetStat(11));
        }

        [Test]
        public void RefreshTwice_ReturnsTheSameValues()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(2, 80);
            owner.AddStat(11, 10);
            owner.AddModifier(new Modifier { StatId = 2, Addend = 0, Multiplier = 20 });
            owner.AddModifier(new Modifier { StatId = 11, Addend = 2, Multiplier = 50 });
            owner.AddModifier(new Modifier { StatId = 2, Addend = 10, Multiplier = 0 });

            owner.RefreshStats();
            owner.RefreshStats();

            Assert.AreEqual(108, owner.GetStat(2).CurrentValue);
            Assert.AreEqual(18, owner.GetStat(11).CurrentValue);
        }
    }
}
