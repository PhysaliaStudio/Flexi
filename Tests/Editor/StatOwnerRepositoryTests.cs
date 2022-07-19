using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.Stats.Tests
{
    public class StatOwnerRepositoryTests
    {
        private readonly List<StatDefinition> validList = new()
        {
            new StatDefinition
            {
                Id = 1,
                Name = "Health"
            },
            new StatDefinition
            {
                Id = 2,
                Name = "MaxHealth"
            },
            new StatDefinition
            {
                Id = 11,
                Name = "Attack"
            },
        };

        [Test]
        public void CreateOwner_TheCreatedOwnerIsManagedByRepository()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(validList);
            var repository = new StatOwnerRepository(table);
            StatOwner owner = repository.CreateOwner();

            Assert.AreSame(owner, repository.GetOwner(owner.Id));
        }

        [Test]
        public void CreateOwner_IsValidReturnsTrue()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(validList);
            var repository = new StatOwnerRepository(table);
            StatOwner owner = repository.CreateOwner();

            Assert.AreEqual(true, owner.IsValid());
        }

        [Test]
        public void DestroyOwner_IsValidReturnsFalse()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(validList);
            var repository = new StatOwnerRepository(table);
            StatOwner owner = repository.CreateOwner();

            owner.Destroy();

            Assert.AreEqual(false, owner.IsValid());
        }

        [Test]
        public void RemoveOwner_TheOwnerIsNull_LogError()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(validList);
            var repository = new StatOwnerRepository(table);

            repository.RemoveOwner(null);

            LogAssert.Expect(LogType.Error, new Regex(".*"));
        }

        [Test]
        public void RemoveOwner_TheOwnerDoesNotBelongToTargetRepository_LogError()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(validList);
            var repository1 = new StatOwnerRepository(table);
            var repository2 = new StatOwnerRepository(table);
            StatOwner ownerFrom1 = repository1.CreateOwner();

            repository2.RemoveOwner(ownerFrom1);

            LogAssert.Expect(LogType.Error, new Regex(".*"));
        }

        [Test]
        public void RemoveOwner_Success_GetOwnerWithTheSameIdReturnsNull()
        {
            StatDefinitionTable table = new StatDefinitionTable.Factory().Create(validList);
            var repository = new StatOwnerRepository(table);
            StatOwner owner = repository.CreateOwner();

            repository.RemoveOwner(owner);

            Assert.IsNull(repository.GetOwner(owner.Id));
        }
    }
}
