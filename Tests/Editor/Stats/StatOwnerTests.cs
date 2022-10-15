using NUnit.Framework;
using UnityEngine;

namespace Physalia.AbilitySystem.Tests
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
        public void SetStatWithoutRefresh_OriginalBaseIs2AndNewBaseIs6_OriginalBaseAndValueAre2AndCurrentBaseIs6()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.SetStat(11, 6);

            var stat = owner.GetStat(11);
            Assert.AreEqual(2, stat.OriginalBase);
            Assert.AreEqual(6, stat.CurrentBase);
            Assert.AreEqual(2, stat.CurrentValue);
        }

        [Test]
        public void AddStat_WithInvalidId_Log2Error()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(999, 2);
            StatTestHelper.LogAssert(LogType.Error);
            StatTestHelper.LogAssert(LogType.Error);
        }

        [Test]
        public void AddStat_WithDuplicatedId_LogError()
        {
            StatOwnerRepository repository = CreateRepository();
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.AddStat(11, 2);
            StatTestHelper.LogAssert(LogType.Error);
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

            var modifierInstance = new StatModifierInstance(modifier);
            owner.AppendModifier(modifierInstance);
            repository.RefreshStats(owner);

            Assert.AreEqual(8, owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void AppendModifier_CurrentIs10AndApplySameInstancesTwice_IsStill8()
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

            var modifierInstance = new StatModifierInstance(modifier);
            owner.AppendModifier(modifierInstance);
            owner.AppendModifier(modifierInstance);
            repository.RefreshStats(owner);

            Assert.AreEqual(8, owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }

        [Test]
        public void AppendModifier_CurrentIs10AndApplyDifferentInstnacesTwice_Becomes6()
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

            var modifierInstance1 = new StatModifierInstance(modifier);
            var modifierInstance2 = new StatModifierInstance(modifier);
            owner.AppendModifier(modifierInstance1);
            owner.AppendModifier(modifierInstance2);
            repository.RefreshStats(owner);

            Assert.AreEqual(6, owner.GetStat(CustomStats.ATTACK).CurrentValue);
        }
    }
}
