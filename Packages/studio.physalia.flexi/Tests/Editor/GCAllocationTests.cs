using System;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace Physalia.Flexi.Tests
{
    public class GCAllocationTests
    {
        private enum StatId
        {
            HealthMax = 100,
        }

        [Test]
        public void AddStatAndGetStat_DoesNotAllocate()
        {
            StatOwnerRepository repository = new StatOwnerRepository();
            StatOwner owner = repository.CreateOwner();

            // Note: The first call to XXXStat allocates CastTo class.
            owner.AddStat(StatId.HealthMax, 2);

            Assert.That(() =>
            {
                for (var i = 0; i < 1000000; i++)
                {
                    _ = owner.GetStat(StatId.HealthMax);
                }
            }, Is.Not.AllocatingGCMemory());
        }

        [Test]
        public void ConvertToInt32_DoesAllocate()
        {
            Assert.That(() =>
            {
                for (var i = 0; i < 1000000; i++)
                {
                    _ = Convert.ToInt32(StatId.HealthMax);
                }
            }, Is.AllocatingGCMemory());
        }

        [Test]
        public void CastToIntFromEnum_DoesNotAllocate()
        {
            // Note: The first call to CastTo.From allocates static classes.
            _ = CastTo<int>.From(StatId.HealthMax);

            Assert.That(() =>
            {
                for (var i = 0; i < 1000000; i++)
                {
                    _ = CastTo<int>.From(StatId.HealthMax);
                }
            }, Is.Not.AllocatingGCMemory());
        }
    }
}
