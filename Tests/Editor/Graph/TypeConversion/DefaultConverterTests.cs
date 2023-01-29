using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Physalia.Flexi.Tests
{
    public class DefaultConverterTests
    {
        public class TestA { }

        public class TestB { }

        public class TestA1 : TestA { }

        [Test]
        public void SingleToSingle_DifferentType_ReturnsNull()
        {
            Func<object, object> converter = ConversionUtility.CreateConverterByDefault<TestA1, TestB>();
            Assert.AreEqual(null, converter);
        }

        [Test]
        public void SingleToSingle_DifferentTypeButAssignableTo_ReturnsUsable()
        {
            Func<object, object> converter = ConversionUtility.CreateConverterByDefault<TestA1, TestA>();
            var value = new TestA1();
            var result = converter.Invoke(value) as TestA;

            Assert.AreEqual(value, result);
        }

        [Test]
        public void SingleToList_MatchGenericType_ReturnsTrue()
        {
            Func<object, object> converter = ConversionUtility.CreateConverterByDefault<TestA1, IList<TestA1>>();
            var value = new TestA1();
            var result = converter.Invoke(value) as IList<TestA1>;

            Assert.AreEqual(value, result[0]);
        }

        [Test]
        public void SingleToList_MatchGenericTypeButInputIsNull_ListCountReturns0()
        {
            Func<object, object> converter = ConversionUtility.CreateConverterByDefault<TestA1, IList<TestA1>>();
            var result = converter.Invoke(null) as IList<TestA1>;
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void SingleToList_NotMatchGenericType_ReturnsNull()
        {
            Func<object, object> converter = ConversionUtility.CreateConverterByDefault<TestA1, IList<TestB>>();
            Assert.AreEqual(null, converter);
        }

        [Test]
        public void SingleToList_NotMatchGenericTypeButAssignableTo_ReturnsUsable()
        {
            Func<object, object> converter = ConversionUtility.CreateConverterByDefault<TestA1, IList<TestA>>();
            var value = new TestA1();
            var result = converter.Invoke(value) as IList<TestA>;

            Assert.AreEqual(value, result[0]);
        }

        [Test]
        public void ListToList_MatchGenericType_ReturnsTrue()
        {
            Func<object, object> converter = ConversionUtility.CreateConverterByDefault<IList<TestA1>, IList<TestA1>>();
            var value = new List<TestA1> { new TestA1(), new TestA1() };
            var result = converter.Invoke(value) as IList<TestA1>;

            Assert.AreEqual(value.Count, result.Count);
            for (var i = 0; i < value.Count; i++)
            {
                Assert.AreEqual(value[i], result[i]);
            }
        }

        [Test]
        public void ListToList_NotMatchGenericType_ReturnsNull()
        {
            Func<object, object> converter = ConversionUtility.CreateConverterByDefault<IList<TestA1>, IList<TestB>>();
            Assert.AreEqual(null, converter);
        }

        [Test]
        public void ListToList_NotMatchGenericTypeButAssignableTo_ReturnsUsable()
        {
            Func<object, object> converter = ConversionUtility.CreateConverterByDefault<IList<TestA1>, IList<TestA>>();
            var value = new List<TestA1> { new TestA1(), new TestA1() };
            var result = converter.Invoke(value) as IList<TestA>;

            Assert.AreEqual(value.Count, result.Count);
            for (var i = 0; i < value.Count; i++)
            {
                Assert.AreEqual(value[i], result[i]);
            }
        }
    }
}
