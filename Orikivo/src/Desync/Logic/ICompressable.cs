using System;

namespace Orikivo.Desync.Logic
{
    public class Locator
    {
        public string Id { get; }

        public float? X { get; }

        public float? Y { get; }
    }

    public class Trip
    {
        public string Id { get; }
        public float? X { get; }
        public float? Y { get; }

        public DateTime StartedAt { get; }

    }

    public interface ICompressable<T>
    {
        byte[] Compress();
        T Decompress();
    }

    /// <summary>
    /// Represents a non-generic object that can be compressed.
    /// </summary>
    public interface ICompressable
    {
        byte[] Compress();
        object Decompress();
    }

    /*
    /// <summary>
    /// Provides methods to compress all value type objects.
    /// </summary>
    public static class Compressor
    {
        // for enums, refer to their derived type. By default, enums are an int.

        // 16 bits, 2 bytes
        public byte[] Compress(ushort value)
        {
            byte[] bytes = new byte[2];
        }

        // 16 bits, 2 bytes
        public byte[] Compress(short value)
        {
            byte[] bytes = new byte[2];
        }

        // 1 bit, 1 byte
        public byte[] Compress(bool value)
        {
            byte[] bytes = new byte[1];
        
        }

        // 16 bits, 2 bytes
        public byte[] Compress(char value)
        {
            byte[] bytes = new byte[2];
        }

        // 32 bits, 4 bytes
        public byte[] Compress(uint value)
        {
            byte[] bytes = new byte[4];
        }

        // 32 bits, 4 bytes
        public byte[] Compress(int value)
        {
            byte[] bytes = new byte[4];
        }

        // 64 bits, 8 bytes
        public byte[] Compress(ulong value)
        {
            byte[] bytes = new byte[8];
        }

        // 64 bits, 8 bytes
        public byte[] Compress(long value)
        {
            byte[] bytes = new byte[8];
        }

        // 32 bits, 4 bytes
        public byte[] Compress(float value)
        {
            byte[] bytes = new byte[4];
        }


        // 64 bits, 8 bytes
        public byte[] Compress(double value)
        {
            byte[] bytes = new byte[8];
        }

        // 128 bits, 16 bytes
        public byte[] Compress(decimal value)
        {
            byte[] bytes = new byte[16];
        }
    }
    */
}