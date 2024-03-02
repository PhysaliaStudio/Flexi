using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Physalia.Flexi.Tests
{
    public class ConversionUtilityTests
    {
        #region Built-in types
        [Test]
        public void Convert_FromByte()
        {
            //Assert.AreEqual((byte)42, ConversionUtility.Convert<byte, byte>(42));
            Assert.AreEqual((sbyte)42, ConversionUtility.Convert<byte, sbyte>(42));
            Assert.AreEqual((short)42, ConversionUtility.Convert<byte, short>(42));
            Assert.AreEqual((ushort)42, ConversionUtility.Convert<byte, ushort>(42));
            Assert.AreEqual(42, ConversionUtility.Convert<byte, int>(42));
            Assert.AreEqual(42U, ConversionUtility.Convert<byte, uint>(42));
            Assert.AreEqual(42L, ConversionUtility.Convert<byte, long>(42));
            Assert.AreEqual(42UL, ConversionUtility.Convert<byte, ulong>(42));
            TestUtilities.AreApproximatelyEqual(42f, ConversionUtility.Convert<byte, float>(42));
            TestUtilities.AreApproximatelyEqual(42d, ConversionUtility.Convert<byte, double>(42));
            TestUtilities.AreApproximatelyEqual(42m, ConversionUtility.Convert<byte, decimal>(42));
        }

        [Test]
        public void Convert_FromSByte()
        {
            Assert.AreEqual((byte)42, ConversionUtility.Convert<sbyte, byte>(42));
            //Assert.AreEqual((sbyte)42, ConversionUtility.Convert<sbyte, sbyte>(42));
            Assert.AreEqual((short)42, ConversionUtility.Convert<sbyte, short>(42));
            Assert.AreEqual((ushort)42, ConversionUtility.Convert<sbyte, ushort>(42));
            Assert.AreEqual(42, ConversionUtility.Convert<sbyte, int>(42));
            Assert.AreEqual(42U, ConversionUtility.Convert<sbyte, uint>(42));
            Assert.AreEqual(42L, ConversionUtility.Convert<sbyte, long>(42));
            Assert.AreEqual(42UL, ConversionUtility.Convert<sbyte, ulong>(42));
            TestUtilities.AreApproximatelyEqual(42f, ConversionUtility.Convert<sbyte, float>(42));
            TestUtilities.AreApproximatelyEqual(42d, ConversionUtility.Convert<sbyte, double>(42));
            TestUtilities.AreApproximatelyEqual(42m, ConversionUtility.Convert<sbyte, decimal>(42));
        }

        [Test]
        public void Convert_FromShort()
        {
            Assert.AreEqual((byte)42, ConversionUtility.Convert<short, byte>(42));
            Assert.AreEqual((sbyte)42, ConversionUtility.Convert<short, sbyte>(42));
            //Assert.AreEqual((short)42, ConversionUtility.Convert<short, short>(42));
            Assert.AreEqual((ushort)42, ConversionUtility.Convert<short, ushort>(42));
            Assert.AreEqual(42, ConversionUtility.Convert<short, int>(42));
            Assert.AreEqual(42U, ConversionUtility.Convert<short, uint>(42));
            Assert.AreEqual(42L, ConversionUtility.Convert<short, long>(42));
            Assert.AreEqual(42UL, ConversionUtility.Convert<short, ulong>(42));
            TestUtilities.AreApproximatelyEqual(42f, ConversionUtility.Convert<short, float>(42));
            TestUtilities.AreApproximatelyEqual(42d, ConversionUtility.Convert<short, double>(42));
            TestUtilities.AreApproximatelyEqual(42m, ConversionUtility.Convert<short, decimal>(42));
        }

        [Test]
        public void Convert_FromUShort()
        {
            Assert.AreEqual((byte)42, ConversionUtility.Convert<ushort, byte>(42));
            Assert.AreEqual((sbyte)42, ConversionUtility.Convert<ushort, sbyte>(42));
            Assert.AreEqual((short)42, ConversionUtility.Convert<ushort, short>(42));
            //Assert.AreEqual((ushort)42, ConversionUtility.Convert<ushort, ushort>(42));
            Assert.AreEqual(42, ConversionUtility.Convert<ushort, int>(42));
            Assert.AreEqual(42U, ConversionUtility.Convert<ushort, uint>(42));
            Assert.AreEqual(42L, ConversionUtility.Convert<ushort, long>(42));
            Assert.AreEqual(42UL, ConversionUtility.Convert<ushort, ulong>(42));
            TestUtilities.AreApproximatelyEqual(42f, ConversionUtility.Convert<ushort, float>(42));
            TestUtilities.AreApproximatelyEqual(42d, ConversionUtility.Convert<ushort, double>(42));
            TestUtilities.AreApproximatelyEqual(42m, ConversionUtility.Convert<ushort, decimal>(42));
        }

        [Test]
        public void Convert_FromInt()
        {
            Assert.AreEqual((byte)42, ConversionUtility.Convert<int, byte>(42));
            Assert.AreEqual((sbyte)42, ConversionUtility.Convert<int, sbyte>(42));
            Assert.AreEqual((short)42, ConversionUtility.Convert<int, short>(42));
            Assert.AreEqual((ushort)42, ConversionUtility.Convert<int, ushort>(42));
            //Assert.AreEqual(42, ConversionUtility.Convert<int, int>(42));
            Assert.AreEqual(42U, ConversionUtility.Convert<int, uint>(42));
            Assert.AreEqual(42L, ConversionUtility.Convert<int, long>(42));
            Assert.AreEqual(42UL, ConversionUtility.Convert<int, ulong>(42));
            TestUtilities.AreApproximatelyEqual(42f, ConversionUtility.Convert<int, float>(42));
            TestUtilities.AreApproximatelyEqual(42d, ConversionUtility.Convert<int, double>(42));
            TestUtilities.AreApproximatelyEqual(42m, ConversionUtility.Convert<int, decimal>(42));
        }

        [Test]
        public void Convert_FromUInt()
        {
            Assert.AreEqual((byte)42, ConversionUtility.Convert<uint, byte>(42));
            Assert.AreEqual((sbyte)42, ConversionUtility.Convert<uint, sbyte>(42));
            Assert.AreEqual((short)42, ConversionUtility.Convert<uint, short>(42));
            Assert.AreEqual((ushort)42, ConversionUtility.Convert<uint, ushort>(42));
            Assert.AreEqual(42, ConversionUtility.Convert<uint, int>(42));
            //Assert.AreEqual(42U, ConversionUtility.Convert<uint, uint>(42));
            Assert.AreEqual(42L, ConversionUtility.Convert<uint, long>(42));
            Assert.AreEqual(42UL, ConversionUtility.Convert<uint, ulong>(42));
            TestUtilities.AreApproximatelyEqual(42f, ConversionUtility.Convert<uint, float>(42));
            TestUtilities.AreApproximatelyEqual(42d, ConversionUtility.Convert<uint, double>(42));
            TestUtilities.AreApproximatelyEqual(42m, ConversionUtility.Convert<uint, decimal>(42));
        }

        [Test]
        public void Convert_FromLong()
        {
            Assert.AreEqual((byte)42, ConversionUtility.Convert<long, byte>(42));
            Assert.AreEqual((sbyte)42, ConversionUtility.Convert<long, sbyte>(42));
            Assert.AreEqual((short)42, ConversionUtility.Convert<long, short>(42));
            Assert.AreEqual((ushort)42, ConversionUtility.Convert<long, ushort>(42));
            Assert.AreEqual(42, ConversionUtility.Convert<long, int>(42));
            Assert.AreEqual(42U, ConversionUtility.Convert<long, uint>(42));
            //Assert.AreEqual(42L, ConversionUtility.Convert<long, long>(42));
            Assert.AreEqual(42UL, ConversionUtility.Convert<long, ulong>(42));
            TestUtilities.AreApproximatelyEqual(42f, ConversionUtility.Convert<long, float>(42));
            TestUtilities.AreApproximatelyEqual(42d, ConversionUtility.Convert<long, double>(42));
            TestUtilities.AreApproximatelyEqual(42m, ConversionUtility.Convert<long, decimal>(42));
        }

        [Test]
        public void Convert_FromULong()
        {
            Assert.AreEqual((byte)42, ConversionUtility.Convert<ulong, byte>(42));
            Assert.AreEqual((sbyte)42, ConversionUtility.Convert<ulong, sbyte>(42));
            Assert.AreEqual((short)42, ConversionUtility.Convert<ulong, short>(42));
            Assert.AreEqual((ushort)42, ConversionUtility.Convert<ulong, ushort>(42));
            Assert.AreEqual(42, ConversionUtility.Convert<ulong, int>(42));
            Assert.AreEqual(42U, ConversionUtility.Convert<ulong, uint>(42));
            Assert.AreEqual(42L, ConversionUtility.Convert<ulong, long>(42));
            //Assert.AreEqual(42UL, ConversionUtility.Convert<ulong, ulong>(42));
            TestUtilities.AreApproximatelyEqual(42f, ConversionUtility.Convert<ulong, float>(42));
            TestUtilities.AreApproximatelyEqual(42d, ConversionUtility.Convert<ulong, double>(42));
            TestUtilities.AreApproximatelyEqual(42m, ConversionUtility.Convert<ulong, decimal>(42));
        }

        [Test]
        public void Convert_FromFloat()
        {
            Assert.AreEqual((byte)3, ConversionUtility.Convert<float, byte>(3.3f));
            Assert.AreEqual((sbyte)3, ConversionUtility.Convert<float, sbyte>(3.3f));
            Assert.AreEqual((short)3, ConversionUtility.Convert<float, short>(3.3f));
            Assert.AreEqual((ushort)3, ConversionUtility.Convert<float, ushort>(3.3f));
            Assert.AreEqual(3, ConversionUtility.Convert<float, int>(3.3f));
            Assert.AreEqual(3U, ConversionUtility.Convert<float, uint>(3.3f));
            Assert.AreEqual(3L, ConversionUtility.Convert<float, long>(3.3f));
            Assert.AreEqual(3UL, ConversionUtility.Convert<float, ulong>(3.3f));
            //TestUtilities.AreApproximatelyEqual(3.3f, ConversionUtility.Convert<float, float>(3.3f));
            TestUtilities.AreApproximatelyEqual(3.3d, ConversionUtility.Convert<float, double>(3.3f));
            TestUtilities.AreApproximatelyEqual(3.3m, ConversionUtility.Convert<float, decimal>(3.3f));
        }

        [Test]
        public void Convert_FromDouble()
        {
            Assert.AreEqual((byte)3, ConversionUtility.Convert<double, byte>(3.3d));
            Assert.AreEqual((sbyte)3, ConversionUtility.Convert<double, sbyte>(3.3d));
            Assert.AreEqual((short)3, ConversionUtility.Convert<double, short>(3.3d));
            Assert.AreEqual((ushort)3, ConversionUtility.Convert<double, ushort>(3.3d));
            Assert.AreEqual(3, ConversionUtility.Convert<double, int>(3.3d));
            Assert.AreEqual(3U, ConversionUtility.Convert<double, uint>(3.3d));
            Assert.AreEqual(3L, ConversionUtility.Convert<double, long>(3.3d));
            Assert.AreEqual(3UL, ConversionUtility.Convert<double, ulong>(3.3d));
            TestUtilities.AreApproximatelyEqual(3.3f, ConversionUtility.Convert<double, float>(3.3d));
            //TestUtilities.AreApproximatelyEqual(3.3d, ConversionUtility.Convert<double, double>(3.3d));
            TestUtilities.AreApproximatelyEqual(3.3m, ConversionUtility.Convert<double, decimal>(3.3d));
        }

        [Test]
        public void Convert_FromDecimal()
        {
            Assert.AreEqual((byte)3, ConversionUtility.Convert<decimal, byte>(3.3m));
            Assert.AreEqual((sbyte)3, ConversionUtility.Convert<decimal, sbyte>(3.3m));
            Assert.AreEqual((short)3, ConversionUtility.Convert<decimal, short>(3.3m));
            Assert.AreEqual((ushort)3, ConversionUtility.Convert<decimal, ushort>(3.3m));
            Assert.AreEqual(3, ConversionUtility.Convert<decimal, int>(3.3m));
            Assert.AreEqual(3U, ConversionUtility.Convert<decimal, uint>(3.3m));
            Assert.AreEqual(3L, ConversionUtility.Convert<decimal, long>(3.3m));
            Assert.AreEqual(3UL, ConversionUtility.Convert<decimal, ulong>(3.3m));
            TestUtilities.AreApproximatelyEqual(3.3f, ConversionUtility.Convert<decimal, float>(3.3m));
            TestUtilities.AreApproximatelyEqual(3.3d, ConversionUtility.Convert<decimal, double>(3.3m));
            //TestUtilities.AreApproximatelyEqual(3.3m, ConversionUtility.Convert<decimal, decimal>(3.3m));
        }
        #endregion

        #region Unity vector types
        [Test]
        public void Convert_FromVector2()
        {
            //TestUtilities.AreApproximatelyEqual(new Vector2(3.3f, 7.7f), ConversionUtility.Convert<Vector2, Vector2>(new Vector2(3.3f, 7.7f)));
            TestUtilities.AreApproximatelyEqual(new Vector3(3.3f, 7.7f), ConversionUtility.Convert<Vector2, Vector3>(new Vector2(3.3f, 7.7f)));
            TestUtilities.AreApproximatelyEqual(new Vector4(3.3f, 7.7f), ConversionUtility.Convert<Vector2, Vector4>(new Vector2(3.3f, 7.7f)));
            Assert.AreEqual(new Vector2Int(3, 7), ConversionUtility.Convert<Vector2, Vector2Int>(new Vector2(3.3f, 7.7f)));
            Assert.AreEqual(new Vector3Int(3, 7), ConversionUtility.Convert<Vector2, Vector3Int>(new Vector2(3.3f, 7.7f)));
        }

        [Test]
        public void Convert_FromVector3()
        {
            TestUtilities.AreApproximatelyEqual(new Vector2(3.3f, 7.7f), ConversionUtility.Convert<Vector3, Vector2>(new Vector3(3.3f, 7.7f, 9.9f)));
            //TestUtilities.AreApproximatelyEqual(new Vector3(3.3f, 7.7f, 9.9f), ConversionUtility.Convert<Vector3, Vector3>(new Vector3(3.3f, 7.7f, 9.9f)));
            TestUtilities.AreApproximatelyEqual(new Vector4(3.3f, 7.7f, 9.9f), ConversionUtility.Convert<Vector3, Vector4>(new Vector3(3.3f, 7.7f, 9.9f)));
            Assert.AreEqual(new Vector2Int(3, 7), ConversionUtility.Convert<Vector3, Vector2Int>(new Vector3(3.3f, 7.7f, 9.9f)));
            Assert.AreEqual(new Vector3Int(3, 7, 9), ConversionUtility.Convert<Vector3, Vector3Int>(new Vector3(3.3f, 7.7f, 9.9f)));
        }

        [Test]
        public void Convert_FromVector4()
        {
            TestUtilities.AreApproximatelyEqual(new Vector2(3.3f, 7.7f), ConversionUtility.Convert<Vector4, Vector2>(new Vector4(3.3f, 7.7f, 9.9f, 11.11f)));
            TestUtilities.AreApproximatelyEqual(new Vector3(3.3f, 7.7f, 9.9f), ConversionUtility.Convert<Vector4, Vector3>(new Vector4(3.3f, 7.7f, 9.9f, 11.11f)));
            //TestUtilities.AreApproximatelyEqual(new Vector4(3.3f, 7.7f, 9.9f, 11.11f), ConversionUtility.Convert<Vector4, Vector4>(new Vector4(3.3f, 7.7f, 9.9f, 11.11f)));
        }

        [Test]
        public void Convert_FromVector2Int()
        {
            TestUtilities.AreApproximatelyEqual(new Vector2(3f, 7f), ConversionUtility.Convert<Vector2Int, Vector2>(new Vector2Int(3, 7)));
            TestUtilities.AreApproximatelyEqual(new Vector3(3f, 7f), ConversionUtility.Convert<Vector2Int, Vector3>(new Vector2Int(3, 7)));
            //TestUtilities.AreApproximatelyEqual(new Vector2Int(3, 7), ConversionUtility.Convert<Vector2Int, Vector2Int>(new Vector2Int(3, 7)));
            TestUtilities.AreApproximatelyEqual(new Vector3Int(3, 7), ConversionUtility.Convert<Vector2Int, Vector3Int>(new Vector2Int(3, 7)));
        }

        [Test]
        public void Convert_FromVector3Int()
        {
            TestUtilities.AreApproximatelyEqual(new Vector2(3f, 7f), ConversionUtility.Convert<Vector3Int, Vector2>(new Vector3Int(3, 7, 9)));
            TestUtilities.AreApproximatelyEqual(new Vector3(3f, 7f, 9f), ConversionUtility.Convert<Vector3Int, Vector3>(new Vector3Int(3, 7, 9)));
            TestUtilities.AreApproximatelyEqual(new Vector2Int(3, 7), ConversionUtility.Convert<Vector3Int, Vector2Int>(new Vector3Int(3, 7, 9)));
            //TestUtilities.AreApproximatelyEqual(new Vector3Int(3, 7, 9), ConversionUtility.Convert<Vector3Int, Vector3Int>(new Vector3Int(3, 7, 9)));
        }
        #endregion

        #region ToString
        [Test]
        public void Convert_ToString_BuiltInTypes()
        {
            Assert.AreEqual("True", ConversionUtility.Convert<bool, string>(true));
            Assert.AreEqual("c", ConversionUtility.Convert<char, string>('c'));
            Assert.AreEqual("42", ConversionUtility.Convert<string, string>("42"));

            Assert.AreEqual("42", ConversionUtility.Convert<byte, string>(42));
            Assert.AreEqual("42", ConversionUtility.Convert<sbyte, string>(42));
            Assert.AreEqual("42", ConversionUtility.Convert<short, string>(42));
            Assert.AreEqual("42", ConversionUtility.Convert<ushort, string>(42));
            Assert.AreEqual("42", ConversionUtility.Convert<int, string>(42));
            Assert.AreEqual("42", ConversionUtility.Convert<uint, string>(42));
            Assert.AreEqual("42", ConversionUtility.Convert<long, string>(42));
            Assert.AreEqual("42", ConversionUtility.Convert<ulong, string>(42));

            Assert.AreEqual("3.3", ConversionUtility.Convert<float, string>(3.3f));
            Assert.AreEqual("3.3", ConversionUtility.Convert<double, string>(3.3d));
            Assert.AreEqual("3.3", ConversionUtility.Convert<decimal, string>(3.3m));
        }

        [Test]
        public void Convert_ToString_UnityVectorTypes()
        {
            Assert.AreEqual("(3.30, 7.70)", ConversionUtility.Convert<Vector2, string>(new Vector2(3.3f, 7.7f)));
            Assert.AreEqual("(3.30, 7.70, 9.90)", ConversionUtility.Convert<Vector3, string>(new Vector3(3.3f, 7.7f, 9.9f)));
            Assert.AreEqual("(3.30, 7.70, 9.90, 11.11)", ConversionUtility.Convert<Vector4, string>(new Vector4(3.3f, 7.7f, 9.9f, 11.11f)));
            Assert.AreEqual("(3, 7)", ConversionUtility.Convert<Vector2Int, string>(new Vector2Int(3, 7)));
            Assert.AreEqual("(3, 7, 9)", ConversionUtility.Convert<Vector3Int, string>(new Vector3Int(3, 7, 9)));
        }

        [Test]
        public void Convert_ToString_EnumarableTypes()
        {
            var list = new List<int> { 3, 7, 9 };
            Assert.AreEqual("[3, 7, 9]", ConversionUtility.Convert<List<int>, string>(list));

            var array = new int[] { 3, 7, 9 };
            Assert.AreEqual("[3, 7, 9]", ConversionUtility.Convert<int[], string>(array));

            var hashSet = new HashSet<int> { 3, 7, 9 };
            Assert.AreEqual("[3, 7, 9]", ConversionUtility.Convert<HashSet<int>, string>(hashSet));

            var dictionary = new Dictionary<int, string> { { 3, "3" }, { 7, "7" }, { 9, "9" } };
            Assert.AreEqual("[[3, 3], [7, 7], [9, 9]]", ConversionUtility.Convert<Dictionary<int, string>, string>(dictionary));
        }

        [Test]
        public void Convert_ToString_AnyOtherTypes()
        {
            var guid = System.Guid.NewGuid();
            Assert.AreEqual(guid.ToString(), ConversionUtility.Convert<System.Guid, string>(guid));

            var color = new Color(0.3f, 0.7f, 0.9f, 1.0f);
            Assert.AreEqual(color.ToString(), ConversionUtility.Convert<Color, string>(color));
        }
        #endregion

        [Test]
        public void Convert_Special_IntToListOfInt()
        {
            List<int> result = ConversionUtility.Convert<int, List<int>>(42);
            TestUtilities.AreListEqual(new List<int> { 42 }, result);
        }

        [Test]
        public void Convert_Special_FloatToListOfInt()
        {
            List<int> result = ConversionUtility.Convert<float, List<int>>(3.3f);
            TestUtilities.AreListEqual(new List<int> { 3 }, result);
        }

        [Test]
        public void Convert_Special_ListOfFloatToListOfInt()
        {
            List<int> result = ConversionUtility.Convert<List<float>, List<int>>(new List<float> { 3.3f, 5.5f });
            TestUtilities.AreListEqual(new List<int> { 3, 5 }, result);
        }

        [Test]
        public void Convert_Special_NullToListOfObject()
        {
            List<object> result = ConversionUtility.Convert<object, List<object>>(null);
            TestUtilities.AreListEqual(new List<object>(), result);
        }

        [Test]
        public void CreateDefaultInstance_Int_Returns0()
        {
            int result = ConversionUtility.CreateDefaultInstance<int>();
            Assert.AreEqual(0, result);
        }

        [Test]
        public void CreateDefaultInstance_Object_ReturnsNull()
        {
            object result = ConversionUtility.CreateDefaultInstance<object>();
            Assert.AreEqual(null, result);
        }

        [Test]
        public void CreateDefaultInstance_ListOfInt_ReturnsEmptyListOfInt()
        {
            IReadOnlyList<int> result = ConversionUtility.CreateDefaultInstance<IReadOnlyList<int>>();
            TestUtilities.AreListEqual(new List<int>(), result as List<int>);
        }
    }
}
