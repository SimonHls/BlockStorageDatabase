using BlockStorageCore.Constants;

namespace BlockStorageCore.Helpers;

/// <summary>
/// Helper methods which convert byte streams into various data types ind vise versa.
/// </summary>
public static class BufferHelper {
    public static Guid ReadBufferGuid(byte[] buffer, int bufferOffset) {
        var guidBuffer = new byte[ByteLengths.GuidLen];
        Buffer.BlockCopy(buffer, bufferOffset, guidBuffer, 0, ByteLengths.GuidLen);
        return new Guid(guidBuffer);
    }

    public static uint ReadBufferUInt32(byte[] buffer, int bufferOffset) {
        var uintBuffer = new byte[ByteLengths.UInt32Len];
        Buffer.BlockCopy(buffer, bufferOffset, uintBuffer, 0, ByteLengths.UInt32Len);
        return LeByteConverter.GetUInt32(uintBuffer);
    }

    public static int ReadBufferInt32(byte[] buffer, int bufferOffset) {
        var intBuffer = new byte[ByteLengths.Int32Len];
        Buffer.BlockCopy(buffer, bufferOffset, intBuffer, 0, ByteLengths.Int32Len);
        return LeByteConverter.GetInt32(intBuffer);
    }

    public static long ReadBufferInt64(byte[] buffer, int bufferOffset) {
        var longBuffer = new byte[ByteLengths.Int64Len];
        Buffer.BlockCopy(buffer, bufferOffset, longBuffer, 0, ByteLengths.Int64Len);
        return LeByteConverter.GetInt64(longBuffer);
    }

    public static double ReadBufferDouble(byte[] buffer, int bufferOffset) {
        var doubleBuffer = new byte[ByteLengths.DoubleLen];
        Buffer.BlockCopy(buffer, bufferOffset, doubleBuffer, 0, ByteLengths.DoubleLen);
        return LeByteConverter.GetDouble(doubleBuffer);
    }

    public static void WriteBuffer(double value, byte[] buffer, int bufferOffset) {
        Buffer.BlockCopy(LeByteConverter.GetBytes(value), 0, buffer, bufferOffset, ByteLengths.DoubleLen);
    }

    public static void WriteBuffer(uint value, byte[] buffer, int bufferOffset) {
        Buffer.BlockCopy(LeByteConverter.GetBytes(value), 0, buffer, bufferOffset, ByteLengths.UInt32Len);
    }

    public static void WriteBuffer(long value, byte[] buffer, int bufferOffset) {
        Buffer.BlockCopy(LeByteConverter.GetBytes(value), 0, buffer, bufferOffset, ByteLengths.LongLen);
    }

    public static void WriteBuffer(int value, byte[] buffer, int bufferOffset) {
        Buffer.BlockCopy(LeByteConverter.GetBytes((int)value), 0, buffer, bufferOffset, ByteLengths.Int32Len);
    }

    public static void WriteBuffer(Guid value, byte[] buffer, int bufferOffset) {
        Buffer.BlockCopy(value.ToByteArray(), 0, buffer, bufferOffset, ByteLengths.GuidLen);
    }

    public static int GetByteLength(string s) {
        return System.Text.Encoding.UTF8.GetByteCount(s);
    }
}