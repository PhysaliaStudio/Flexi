using NUnit.Framework;
using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public class StatOwnerTests
    {
        private StatOwnerRepository CreateRepository()
        {
            StatDefinitionListAsset statDefinitionList = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            StatOwnerRepository repository = StatOwnerRepository.Create(statDefinitionList);
            return repository;
        }

        [Test]
        public void AddStat_OriginalValueIs2_GetStatReturnsNotNullAndBothBaseAndValueAre2()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);

            var stat = owner.GetStat(11);
            Assert.IsNotNull(stat);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(2, stat.CurrentBase);
            Assert.AreEqual(2, stat.CurrentValue);
        }

        [Test]
        public void AddStat_WithInvalidId_Log2Error()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(999, 2);
            TestUtilities.LogAssertAnyString(LogType.Error);
            TestUtilities.LogAssertAnyString(LogType.Error);
        }

        [Test]
        public void AddStat_WithDuplicatedId_LogError()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.AddStat(11, 2);
            TestUtilities.LogAssertAnyString(LogType.Error);
        }

        [Test]
        public void RemoveStat_AddedStat_GetStatReturnsNull()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.RemoveStat(11);

            Assert.IsNull(owner.GetStat(11));
        }

        [Test]
        public void GetStat_WithNotAddedId_ReturnsNull()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            Assert.IsNull(owner.GetStat(11));
        }

        [Test]
        public void SetStat_OriginalBaseIs2AndNewBaseIs6_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.SetStat(11, 6);

            var stat = owner.GetStat(11);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }

        [Test]
        public void ModifyStat_OriginalBaseIs2AndAdd4_OriginalBaseIs2AndCurrentBaseAndCurrentValueAre6()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.ModifyStat(11, 4);

            var stat = owner.GetStat(11);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(6, stat.CurrentValue);
        }

        [Test]
        public void AppendModifier_CurrentIs10_Becomes8()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();
            owner.AddStat(CustomStats.ATTACK, 10);

            var modifier = new StatModifier();
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.ADD,
                value = -2,
            });

            owner.AppendModifier(modifier);
            repository.RefreshStats(owner);

            Assert.AreEqual(8, owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void AppendModifier_CurrentIs10AndApplyTwice_Becomes6()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();
            owner.AddStat(CustomStats.ATTACK, 10);

            var modifier = new StatModifier();
            modifier.items.Add(new StatModifierItem
            {
                statId = StatTestHelper.ATTACK,
                op = StatModifierItem.Operator.ADD,
                value = -2,
            });

            owner.AppendModifier(modifier);
            owner.AppendModifier(modifier);
            repository.RefreshStats(owner);

            Assert.AreEqual(6, owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }
    }
}
