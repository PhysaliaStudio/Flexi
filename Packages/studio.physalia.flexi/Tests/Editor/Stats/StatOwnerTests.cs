using NUnit.Framework;
using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public class StatOwnerTests
    {
        [Test]
        public void AddStat_OriginalValueIs2_GetStatReturnsNotNullAndBothBaseAndValueAre2()
        {
            StatOwnerRepository repository = new StatOwnerRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);

            var stat = owner.GetStat(11);
            Assert.IsNotNull(stat);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(2, stat.CurrentBase);
            Assert.AreEqual(2, stat.CurrentValue);
        }

        [Test]
        public void AddStat_WithDuplicatedId_LogError()
        {
            StatOwnerRepository repository = new StatOwnerRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.AddStat(11, 2);
            TestUtilities.LogAssertAnyString(LogType.Error);
        }

        [Test]
        public void RemoveStat_AddedStat_GetStatReturnsNull()
        {
            StatOwnerRepository repository = new StatOwnerRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.RemoveStat(11);

            Assert.IsNull(owner.GetStat(11));
        }

        [Test]
        public void GetStat_WithNotAddedId_ReturnsNull()
        {
            StatOwnerRepository repository = new StatOwnerRepository();
            StatOwner owner = repository.CreateOwner();

            Assert.IsNull(owner.GetStat(11));
        }
    }
}
