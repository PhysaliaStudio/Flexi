using NUnit.Framework;
using UnityEngine;

namespace Physalia.AbilitySystem.StatSystem.Tests
{
    public class StatOwnerTests
    {
        [Test]
        public void AddStat_OriginalValueIs2_GetStatReturnsNotNullAndBothBaseAndValueAre2()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);

            var stat = owner.GetStat(11);
            Assert.IsNotNull(stat);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(2, stat.CurrentBase);
            Assert.AreEqual(2, stat.CurrentValue);
        }

        [Test]
        public void SetStat_OriginalBaseIs2AndNewBaseIs6_OriginalBaseIs2AndCurrentBaseAndValueAre6()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.SetStat(11, 6);

            var stat = owner.GetStat(11);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }

        [Test]
        public void SetStatWithoutRefresh_OriginalBaseIs2AndNewBaseIs6_OriginalBaseAndValueAre2AndCurrentBaseIs6()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.SetStat(11, 6, false);

            var stat = owner.GetStat(11);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(2, stat.CurrentValue);
        }

        [Test]
        public void AddStat_WithInvalidId_Log2Error()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(999, 2);
            StatTestHelper.LogAssert(LogType.Error);
            StatTestHelper.LogAssert(LogType.Error);
        }

        [Test]
        public void AddStat_WithDuplicatedId_LogError()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.AddStat(11, 2);
            StatTestHelper.LogAssert(LogType.Error);
        }

        [Test]
        public void RemoveStat_AddedStat_GetStatReturnsNull()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.RemoveStat(11);

            Assert.IsNull(owner.GetStat(11));
        }

        [Test]
        public void GetStat_WithNotAddedId_ReturnsNull()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            Assert.IsNull(owner.GetStat(11));
        }
    }
}
