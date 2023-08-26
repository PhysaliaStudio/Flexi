using NUnit.Framework;
using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public class StatOwnerRepositoryTests
    {
        [Test]
        public void CreateOwner_TheCreatedOwnerIsManagedByRepository()
        {
            StatOwnerRepository repository = new StatOwnerRepository();
            StatOwner owner = repository.CreateOwner();

            Assert.AreSame(owner, repository.GetOwner(owner.Id));
        }

        [Test]
        public void CreateOwner_IsValidReturnsTrue()
        {
            StatOwnerRepository repository = new StatOwnerRepository();
            StatOwner owner = repository.CreateOwner();

            Assert.AreEqual(true, owner.IsValid());
        }

        [Test]
        public void DestroyOwner_IsValidReturnsFalse()
        {
            StatOwnerRepository repository = new StatOwnerRepository();
            StatOwner owner = repository.CreateOwner();

            owner.Destroy();

            Assert.AreEqual(false, owner.IsValid());
        }

        [Test]
        public void RemoveOwner_TheOwnerIsNull_LogError()
        {
            StatOwnerRepository repository = new StatOwnerRepository();

            repository.RemoveOwner(null);

            TestUtilities.LogAssertAnyString(LogType.Error);
        }

        [Test]
        public void RemoveOwner_TheOwnerDoesNotBelongToTargetRepository_LogError()
        {
            StatOwnerRepository repository1 = new StatOwnerRepository();
            StatOwnerRepository repository2 = new StatOwnerRepository();
            StatOwner ownerFrom1 = repository1.CreateOwner();

            repository2.RemoveOwner(ownerFrom1);

            TestUtilities.LogAssertAnyString(LogType.Error);
        }

        [Test]
        public void RemoveOwner_Success_GetOwnerWithTheSameIdReturnsNull()
        {
            StatOwnerRepository repository = new StatOwnerRepository();
            StatOwner owner = repository.CreateOwner();

            repository.RemoveOwner(owner);

            Assert.IsNull(repository.GetOwner(owner.Id));
        }
    }
}
