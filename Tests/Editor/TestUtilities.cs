using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace Physalia.AbilityFramework.Tests
{
    internal static class TestUtilities
    {
        private static readonly Regex ANY_STRING = new(".*");

        internal static void LogAssertAnyString(LogType type)
        {
            LogAssert.Expect(type, ANY_STRING);
        }
    }
}
