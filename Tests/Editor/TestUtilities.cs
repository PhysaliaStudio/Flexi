using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.Flexi.Tests
{
    internal static class TestUtilities
    {
        private static readonly Regex ANY_STRING = new(".*");

        internal static void LogAssertAnyString(LogType type)
        {
            LogAssert.Expect(type, ANY_STRING);
        }

        internal static void AreListEqual(IList expected, IList actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i], $"Element {i} is not equal");
            }
        }
    }
}
