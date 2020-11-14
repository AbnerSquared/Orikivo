using System;
using System.IO;

namespace Orikivo
{
    // represents a group of blocks for a specified buffer size
    public class BufferReader<TStream> : IDisposable
        where TStream : Stream
    {
        public BufferReader(TStream stream, int bufferSize)
        {
            stream.Position = 0;
            Stream = stream;
            Value = new byte[bufferSize];
        }

        private Stream Stream { get; }

        public byte[] Value { get; }

        public long ReadBytes { get; private set; }

        public long RemainingBytes => Stream.Length - ReadBytes;

        public long Position
        {
            get => Stream.Position;
            set => Stream.Position = value;
        }

        public bool Disposed { get; private set; }

        // reads the next buffer block
        public int Next()
        {
            if (Disposed)
                throw new ObjectDisposedException("The specified buffer reader has been disposed");

            if (RemainingBytes == 0)
                return 0;

            int readBytes = Stream.Read(Value, 0, Value.Length);
            ReadBytes += readBytes;

            return readBytes;
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            Stream.Dispose();
            Disposed = true;
        }
    }
}
