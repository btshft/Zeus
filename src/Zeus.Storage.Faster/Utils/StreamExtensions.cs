using System.IO;

namespace Zeus.Storage.Faster.Utils
{
    internal static class StreamExtensions
    {
        public static int ReadInt32(this Stream source)
        {
            var value = source.ReadByte() << 24;
            value += source.ReadByte() << 16;
            value += source.ReadByte() << 8;
            value += source.ReadByte();
            return value;
        }

        public static void WriteInt32(this Stream target, int value)
        {
            target.WriteByte((byte)(value >> 24));
            target.WriteByte((byte)(value >> 16));
            target.WriteByte((byte)(value >> 8));
            target.WriteByte((byte)(value));
        }
    }
}