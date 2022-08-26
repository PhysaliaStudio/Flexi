using NUnit.Framework;
using UnityEngine;

namespace Physalia.AbilitySystem.StatSystem.Tests
{
    public class StatOwnerRepositoryTests
    {
        [Test]
        public void CreateOwner_TheCreatedOwnerIsManagedByRepository()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            Assert.AreSame(owner, repository.GetOwner(owner.Id));
        }

        [Test]
        public void CreateOwner_IsValidReturnsTrue()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            Assert.AreEqual(true, owner.IsValid());
        }

        [Test]
        public void DestroyOwner_IsValidReturnsFalse()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            owner.Destroy();

            Assert.AreEqual(false, owner.IsValid());
        }

        [Test]
        public void RemoveOwner_TheOwnerIsNull_LogError()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);

            repository.RemoveOwner(null);

            StatTestHelper.LogAssert(LogType.Error);
        }

        [Test]
        public void RemoveOwner_TheOwnerDoesNotBelongToTargetRepository_LogError()
        {
            StatOwnerRepository repository1 = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwnerRepository repository2 = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner ownerFrom1 = repository1.CreateOwner();

            repository2.RemoveOwner(ownerFrom1);

            StatTestHelper.LogAssert(LogType.Error);
        }

        [Test]
        public void RemoveOwner_Success_GetOwnerWithTheSameIdReturnsNull()
        {
            StatOwnerRepository repository = StatOwnerRepository.Create(StatTestHelper.ValidList);
            StatOwner owner = repository.CreateOwner();

            repository.RemoveOwner(owner);

            Assert.IsNull(repository.GetOwner(owner.Id));
        }
    }
}
