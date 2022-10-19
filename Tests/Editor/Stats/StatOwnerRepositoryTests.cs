using NUnit.Framework;
using UnityEngine;

namespace Physalia.AbilityFramework.Tests
{
    public class StatOwnerRepositoryTests
    {
        [Test]
        public void CreateOwner_TheCreatedOwnerIsManagedByRepository()
        {
            StatDefinitionListAsset statDefinitionList = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            StatOwnerRepository repository = StatOwnerRepository.Create(statDefinitionList);
            StatOwner owner = repository.CreateOwner();

            Assert.AreSame(owner, repository.GetOwner(owner.Id));
        }

        [Test]
        public void CreateOwner_IsValidReturnsTrue()
        {
            StatDefinitionListAsset statDefinitionList = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            StatOwnerRepository repository = StatOwnerRepository.Create(statDefinitionList);
            StatOwner owner = repository.CreateOwner();

            Assert.AreEqual(true, owner.IsValid());
        }

        [Test]
        public void DestroyOwner_IsValidReturnsFalse()
        {
            StatDefinitionListAsset statDefinitionList = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            StatOwnerRepository repository = StatOwnerRepository.Create(statDefinitionList);
            StatOwner owner = repository.CreateOwner();

            owner.Destroy();

            Assert.AreEqual(false, owner.IsValid());
        }

        [Test]
        public void RemoveOwner_TheOwnerIsNull_LogError()
        {
            StatDefinitionListAsset statDefinitionList = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            StatOwnerRepository repository = StatOwnerRepository.Create(statDefinitionList);

            repository.RemoveOwner(null);

            StatTestHelper.LogAssert(LogType.Error);
        }

        [Test]
        public void RemoveOwner_TheOwnerDoesNotBelongToTargetRepository_LogError()
        {
            StatDefinitionListAsset statDefinitionList = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            StatOwnerRepository repository1 = StatOwnerRepository.Create(statDefinitionList);
            StatOwnerRepository repository2 = StatOwnerRepository.Create(statDefinitionList);
            StatOwner ownerFrom1 = repository1.CreateOwner();

            repository2.RemoveOwner(ownerFrom1);

            StatTestHelper.LogAssert(LogType.Error);
        }

        [Test]
        public void RemoveOwner_Success_GetOwnerWithTheSameIdReturnsNull()
        {
            StatDefinitionListAsset statDefinitionList = StatDefinitionListAsset.CreateWithList(StatTestHelper.ValidList);
            StatOwnerRepository repository = StatOwnerRepository.Create(statDefinitionList);
            StatOwner owner = repository.CreateOwner();

            repository.RemoveOwner(owner);

            Assert.IsNull(repository.GetOwner(owner.Id));
        }
    }
}
