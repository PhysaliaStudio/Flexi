using NUnit.Framework;

namespace Physalia.AbilityFramework.Tests
{
    public class CompareOperatorTests
    {
        [Test]
        public void Equal_42And42_ReturnsTrue()
        {
            Assert.AreEqual(true, CompareOperator.EQUAL.Compare(42, 42));
        }

        [Test]
        public void Equal_42And44_ReturnsFalse()
        {
            Assert.AreEqual(false, CompareOperator.EQUAL.Compare(42, 44));
        }

        [Test]
        public void NotEqual_42And42_ReturnsFalse()
        {
            Assert.AreEqual(false, CompareOperator.NOT_EQUAL.Compare(42, 42));
        }

        [Test]
        public void NotEqual_42And44_ReturnsTrue()
        {
            Assert.AreEqual(true, CompareOperator.NOT_EQUAL.Compare(42, 44));
        }

        [Test]
        public void Less_42And42_ReturnsFalse()
        {
            Assert.AreEqual(false, CompareOperator.LESS.Compare(42, 42));
        }

        [Test]
        public void Less_42And44_ReturnsTrue()
        {
            Assert.AreEqual(true, CompareOperator.LESS.Compare(42, 44));
        }

        [Test]
        public void Less_42And40_ReturnsFalse()
        {
            Assert.AreEqual(false, CompareOperator.LESS.Compare(42, 40));
        }

        [Test]
        public void LessOrEqual_42And42_ReturnsTrue()
        {
            Assert.AreEqual(true, CompareOperator.LESS_OR_EQUAL.Compare(42, 42));
        }

        [Test]
        public void LessOrEqual_42And44_ReturnsTrue()
        {
            Assert.AreEqual(true, CompareOperator.LESS_OR_EQUAL.Compare(42, 44));
        }

        [Test]
        public void LessOrEqual_42And40_ReturnsFalse()
        {
            Assert.AreEqual(false, CompareOperator.LESS_OR_EQUAL.Compare(42, 40));
        }

        [Test]
        public void Greater_42And42_ReturnsFalse()
        {
            Assert.AreEqual(false, CompareOperator.GREATER.Compare(42, 42));
        }

        [Test]
        public void Greater_42And44_ReturnsFalse()
        {
            Assert.AreEqual(false, CompareOperator.GREATER.Compare(42, 44));
        }

        [Test]
        public void Greater_42And40_ReturnsTrue()
        {
            Assert.AreEqual(true, CompareOperator.GREATER.Compare(42, 40));
        }

        [Test]
        public void GreaterOrEqual_42And42_ReturnsTrue()
        {
            Assert.AreEqual(true, CompareOperator.GREATER_OR_EQUAL.Compare(42, 42));
        }

        [Test]
        public void GreaterOrEqual_42And44_ReturnsFalse()
        {
            Assert.AreEqual(false, CompareOperator.GREATER_OR_EQUAL.Compare(42, 44));
        }

        [Test]
        public void GreaterOrEqual_42And40_ReturnsTrue()
        {
            Assert.AreEqual(true, CompareOperator.GREATER_OR_EQUAL.Compare(42, 40));
        }
    }
}
