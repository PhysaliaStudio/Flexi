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
            if (expected == actual)
            {
                return;
            }

            if (expected != null && actual == null)
            {
                Assert.Fail($"Expected: List with {expected.Count} elements\n But was: null");
                return;
            }

            if (expected == null && actual != null)
            {
                Assert.Fail($"Expected: null\n But was: List with {actual.Count} elements");
                return;
            }

            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i], $"Element {i} is not equal");
            }
        }

        private static bool ApproximatelyEqual(float a, float b)
        {
            float diff = a - b;
            if (diff < 0)
            {
                diff = -diff;
            }

            bool result = diff < 1e-5;
            return result;
        }

        internal static void AreApproximatelyEqual(float expected, float actual)
        {
            float diff = expected - actual;
            if (diff < 0)
            {
                diff = -diff;
            }

            if (diff > 1e-5)
            {
                Assert.Fail($"Expected: {expected}\n But was: {actual}");
            }
        }

        internal static void AreApproximatelyEqual(double expected, double actual)
        {
            double diff = expected - actual;
            if (diff < 0)
            {
                diff = -diff;
            }

            if (diff > 1e-5)
            {
                Assert.Fail($"Expected: {expected}\n But was: {actual}");
            }
        }

        internal static void AreApproximatelyEqual(decimal expected, decimal actual)
        {
            decimal diff = expected - actual;
            if (diff < 0)
            {
                diff = -diff;
            }

            if (diff > 1e-5m)
            {
                Assert.Fail($"Expected: {expected}\n But was: {actual}");
            }
        }

        internal static void AreApproximatelyEqual(Vector2 expected, Vector2 actual)
        {
            bool resultX = ApproximatelyEqual(expected.x, actual.x);
            bool resultY = ApproximatelyEqual(expected.y, actual.y);
            if (!resultX || !resultY)
            {
                Assert.Fail($"Expected: ({expected.x}, {expected.y})\n But was: ({actual.x}, {actual.y})");
            }
        }

        internal static void AreApproximatelyEqual(Vector3 expected, Vector3 actual)
        {
            bool resultX = ApproximatelyEqual(expected.x, actual.x);
            bool resultY = ApproximatelyEqual(expected.y, actual.y);
            bool resultZ = ApproximatelyEqual(expected.z, actual.z);
            if (!resultX || !resultY || !resultZ)
            {
                Assert.Fail($"Expected: ({expected.x}, {expected.y}, {expected.x})\n But was: ({actual.x}, {actual.y}, {actual.z})");
            }
        }

        internal static void AreApproximatelyEqual(Vector4 expected, Vector4 actual)
        {
            bool resultX = ApproximatelyEqual(expected.x, actual.x);
            bool resultY = ApproximatelyEqual(expected.y, actual.y);
            bool resultZ = ApproximatelyEqual(expected.z, actual.z);
            bool resultW = ApproximatelyEqual(expected.w, actual.w);
            if (!resultX || !resultY || !resultZ || !resultW)
            {
                Assert.Fail($"Expected: ({expected.x}, {expected.y}, {expected.z}, {expected.w})\n But was: ({actual.x}, {actual.y}, {actual.z}, {actual.w})");
            }
        }
    }
}
