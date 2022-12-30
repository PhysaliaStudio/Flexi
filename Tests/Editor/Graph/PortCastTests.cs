using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public class PortCastTests
    {
        [Test]
        public void AnyToMissing_ReturnsFalse()
        {
            bool result = Port.CanPortCast(typeof(BoxCollider), typeof(Missing));
            Assert.AreEqual(false, result);
        }

        [Test]
        public void MissingToAny_ReturnsFalse()
        {
            bool result = Port.CanPortCast(typeof(Missing), typeof(BoxCollider));
            Assert.AreEqual(false, result);
        }

        [Test]
        public void MissingToMissing_ReturnsFalse()
        {
            bool result = Port.CanPortCast(typeof(Missing), typeof(Missing));
            Assert.AreEqual(false, result);
        }

        [Test]
        public void SingleToSingle_SameType_ReturnsTrue()
        {
            bool result = Port.CanPortCast(typeof(BoxCollider), typeof(BoxCollider));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void SingleToSingle_DifferentType_ReturnsFalse()
        {
            bool result = Port.CanPortCast(typeof(BoxCollider), typeof(SphereCollider));
            Assert.AreEqual(false, result);
        }

        [Test]
        public void SingleToSingle_DifferentTypeButAssignableTo_ReturnsTrue()
        {
            bool result = Port.CanPortCast(typeof(BoxCollider), typeof(Collider));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void SingleToList_MatchGenericType_ReturnsTrue()
        {
            bool result = Port.CanPortCast(typeof(BoxCollider), typeof(IList<BoxCollider>));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void SingleToList_NotMatchGenericType_ReturnsFalse()
        {
            bool result = Port.CanPortCast(typeof(BoxCollider), typeof(IList<SphereCollider>));
            Assert.AreEqual(false, result);
        }

        [Test]
        public void SingleToList_NotMatchGenericTypeButAssignableTo_ReturnsTrue()
        {
            bool result = Port.CanPortCast(typeof(BoxCollider), typeof(IList<Collider>));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListToSingle_MatchGenericType_ReturnsFalse()
        {
            bool result = Port.CanPortCast(typeof(IList<BoxCollider>), typeof(BoxCollider));
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ListToList_MatchGenericType_ReturnsTrue()
        {
            bool result = Port.CanPortCast(typeof(IList<BoxCollider>), typeof(IList<BoxCollider>));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListToList_NotMatchGenericType_ReturnsFalse()
        {
            bool result = Port.CanPortCast(typeof(IList<BoxCollider>), typeof(IList<SphereCollider>));
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ListToList_NotMatchGenericTypeButAssignableTo_ReturnsTrue()
        {
            bool result = Port.CanPortCast(typeof(IList<BoxCollider>), typeof(IList<Collider>));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListToList_ListOfSubClassToIReadOnlyListOfClass_ReturnsTrue()
        {
            bool result = Port.CanPortCast(typeof(List<BoxCollider>), typeof(IReadOnlyList<Collider>));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListToCollection_ReturnsTrue()
        {
            bool result = Port.CanPortCast(typeof(List<BoxCollider>), typeof(ICollection));
            Assert.AreEqual(true, result);
        }
    }
}
