namespace Physalia.Flexi
{
    public class DotNetConversionSchema : ConversionSchema
    {
        public override void Handle(IConversionHandler handler)
        {
            //handler.Handle<byte, byte>(value => value);
            handler.Handle<byte, sbyte>(value => (sbyte)value);
            handler.Handle<byte, short>(value => value);
            handler.Handle<byte, ushort>(value => value);
            handler.Handle<byte, int>(value => value);
            handler.Handle<byte, uint>(value => value);
            handler.Handle<byte, long>(value => value);
            handler.Handle<byte, ulong>(value => value);
            handler.Handle<byte, float>(value => value);
            handler.Handle<byte, double>(value => value);
            handler.Handle<byte, decimal>(value => value);

            handler.Handle<sbyte, byte>(value => (byte)value);
            //handler.Handle<sbyte, sbyte>(value => value);
            handler.Handle<sbyte, short>(value => value);
            handler.Handle<sbyte, ushort>(value => (ushort)value);
            handler.Handle<sbyte, int>(value => value);
            handler.Handle<sbyte, uint>(value => (uint)value);
            handler.Handle<sbyte, long>(value => value);
            handler.Handle<sbyte, ulong>(value => (ulong)value);
            handler.Handle<sbyte, float>(value => value);
            handler.Handle<sbyte, double>(value => value);
            handler.Handle<sbyte, decimal>(value => value);

            handler.Handle<short, byte>(value => (byte)value);
            handler.Handle<short, sbyte>(value => (sbyte)value);
            //handler.Handle<short, short>(value => value);
            handler.Handle<short, ushort>(value => (ushort)value);
            handler.Handle<short, int>(value => value);
            handler.Handle<short, uint>(value => (uint)value);
            handler.Handle<short, long>(value => value);
            handler.Handle<short, ulong>(value => (ulong)value);
            handler.Handle<short, float>(value => value);
            handler.Handle<short, double>(value => value);
            handler.Handle<short, decimal>(value => value);

            handler.Handle<ushort, byte>(value => (byte)value);
            handler.Handle<ushort, sbyte>(value => (sbyte)value);
            handler.Handle<ushort, short>(value => (short)value);
            //handler.Handle<ushort, ushort>(value => value);
            handler.Handle<ushort, int>(value => value);
            handler.Handle<ushort, uint>(value => value);
            handler.Handle<ushort, long>(value => value);
            handler.Handle<ushort, ulong>(value => value);
            handler.Handle<ushort, float>(value => value);
            handler.Handle<ushort, double>(value => value);
            handler.Handle<ushort, decimal>(value => value);

            handler.Handle<int, byte>(value => (byte)value);
            handler.Handle<int, sbyte>(value => (sbyte)value);
            handler.Handle<int, short>(value => (short)value);
            handler.Handle<int, ushort>(value => (ushort)value);
            //handler.Handle<int, int>(value => value);
            handler.Handle<int, uint>(value => (uint)value);
            handler.Handle<int, long>(value => value);
            handler.Handle<int, ulong>(value => (ulong)value);
            handler.Handle<int, float>(value => value);
            handler.Handle<int, double>(value => value);
            handler.Handle<int, decimal>(value => value);

            handler.Handle<uint, byte>(value => (byte)value);
            handler.Handle<uint, sbyte>(value => (sbyte)value);
            handler.Handle<uint, short>(value => (short)value);
            handler.Handle<uint, ushort>(value => (ushort)value);
            handler.Handle<uint, int>(value => (int)value);
            //handler.Handle<uint, uint>(value => value);
            handler.Handle<uint, long>(value => value);
            handler.Handle<uint, ulong>(value => value);
            handler.Handle<uint, float>(value => value);
            handler.Handle<uint, double>(value => value);
            handler.Handle<uint, decimal>(value => value);

            handler.Handle<long, byte>(value => (byte)value);
            handler.Handle<long, sbyte>(value => (sbyte)value);
            handler.Handle<long, short>(value => (short)value);
            handler.Handle<long, ushort>(value => (ushort)value);
            handler.Handle<long, int>(value => (int)value);
            handler.Handle<long, uint>(value => (uint)value);
            //handler.Handle<long, long>(value => value);
            handler.Handle<long, ulong>(value => (ulong)value);
            handler.Handle<long, float>(value => value);
            handler.Handle<long, double>(value => value);
            handler.Handle<long, decimal>(value => value);

            handler.Handle<ulong, byte>(value => (byte)value);
            handler.Handle<ulong, sbyte>(value => (sbyte)value);
            handler.Handle<ulong, short>(value => (short)value);
            handler.Handle<ulong, ushort>(value => (ushort)value);
            handler.Handle<ulong, int>(value => (int)value);
            handler.Handle<ulong, uint>(value => (uint)value);
            handler.Handle<ulong, long>(value => (long)value);
            //handler.Handle<ulong, ulong>(value => value);
            handler.Handle<ulong, float>(value => value);
            handler.Handle<ulong, double>(value => value);
            handler.Handle<ulong, decimal>(value => value);

            handler.Handle<float, byte>(value => (byte)value);
            handler.Handle<float, sbyte>(value => (sbyte)value);
            handler.Handle<float, short>(value => (short)value);
            handler.Handle<float, ushort>(value => (ushort)value);
            handler.Handle<float, int>(value => (int)value);
            handler.Handle<float, uint>(value => (uint)value);
            handler.Handle<float, long>(value => (long)value);
            handler.Handle<float, ulong>(value => (ulong)value);
            //handler.Handle<float, float>(value => value);
            handler.Handle<float, double>(value => value);
            handler.Handle<float, decimal>(value => (decimal)value);

            handler.Handle<double, byte>(value => (byte)value);
            handler.Handle<double, sbyte>(value => (sbyte)value);
            handler.Handle<double, short>(value => (short)value);
            handler.Handle<double, ushort>(value => (ushort)value);
            handler.Handle<double, int>(value => (int)value);
            handler.Handle<double, uint>(value => (uint)value);
            handler.Handle<double, long>(value => (long)value);
            handler.Handle<double, ulong>(value => (ulong)value);
            handler.Handle<double, float>(value => (float)value);
            //handler.Handle<double, double>(value => value);
            handler.Handle<double, decimal>(value => (decimal)value);

            handler.Handle<decimal, byte>(value => (byte)value);
            handler.Handle<decimal, sbyte>(value => (sbyte)value);
            handler.Handle<decimal, short>(value => (short)value);
            handler.Handle<decimal, ushort>(value => (ushort)value);
            handler.Handle<decimal, int>(value => (int)value);
            handler.Handle<decimal, uint>(value => (uint)value);
            handler.Handle<decimal, long>(value => (long)value);
            handler.Handle<decimal, ulong>(value => (ulong)value);
            handler.Handle<decimal, float>(value => (float)value);
            handler.Handle<decimal, double>(value => (double)value);
            //handler.Handle<decimal, decimal>(value => value);

            handler.Handle<byte, string>(value => value.ToString());
            handler.Handle<sbyte, string>(value => value.ToString());
            handler.Handle<short, string>(value => value.ToString());
            handler.Handle<ushort, string>(value => value.ToString());
            handler.Handle<int, string>(value => value.ToString());
            handler.Handle<uint, string>(value => value.ToString());
            handler.Handle<long, string>(value => value.ToString());
            handler.Handle<ulong, string>(value => value.ToString());
            handler.Handle<float, string>(value => value.ToString());
            handler.Handle<double, string>(value => value.ToString());
            handler.Handle<decimal, string>(value => value.ToString());
        }
    }
}
