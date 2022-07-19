using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.Stats.Tests
{
    public class StatOwnerTests
    {
        [Test]
        public void AddStat_StartValueIs2_GetStatReturnsNotNullAndValueIs2()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(StatTestHelper.ValidList);
            var repository = new StatOwnerRepository(table);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);

            var stat = owner.GetStat(11);
            Assert.IsNotNull(stat);
            Assert.AreEqual(2, stat.Value);
        }

        [Test]
        public void AddStat_WithInvalidId_Log2Error()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(StatTestHelper.ValidList);
            var repository = new StatOwnerRepository(table);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(999, 2);
            LogAssert.Expect(LogType.Error, new Regex(".*"));
            LogAssert.Expect(LogType.Error, new Regex(".*"));
        }

        [Test]
        public void AddStat_WithDuplicatedId_LogError()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(StatTestHelper.ValidList);
            var repository = new StatOwnerRepository(table);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.AddStat(11, 2);
            LogAssert.Expect(LogType.Error, new Regex(".*"));
        }

        [Test]
        public void RemoveStat_AddedStat_GetStatReturnsNull()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(StatTestHelper.ValidList);
            var repository = new StatOwnerRepository(table);
            StatOwner owner = repository.CreateOwner();

            owner.AddStat(11, 2);
            owner.RemoveStat(11);

            Assert.IsNull(owner.GetStat(11));
        }

        [Test]
        public void GetStat_WithNotAddedId_ReturnsNull()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(StatTestHelper.ValidList);
            var repository = new StatOwnerRepository(table);
            StatOwner owner = repository.CreateOwner();

            Assert.IsNull(owner.GetStat(11));
        }
    }
}
