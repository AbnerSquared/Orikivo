using System;
using System.IO;

namespace Orikivo
{
    /// <summary>
    /// Represents a temporary serialized <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <typeparam name="TStream"></typeparam>
    public class Temporary<TStream> : IDisposable where TStream : Stream
    {
        private string _tmpPath;

        public Temporary(TStream stream)
        {
            _tmpPath = Path.GetTempFileName();
            Stream = stream;
        }

        private TStream _stream;
        public TStream Stream
        {
            get
            {
                return _stream;
            }
            set
            {
                _stream = value;
                Update();
            }
        }

        // Updates the FileStream for the TEMPORARY PATH.
        // Read()
        public MemoryStream Read()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(_tmpPath));

            MemoryStream memory = new MemoryStream();

            using (FileStream stream = File.OpenRead(_tmpPath))
                stream.CopyTo(memory);

            return memory;
        }

        public void Update()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(_tmpPath));

            using (FileStream writer = new FileStream(_tmpPath, FileMode.Open, FileAccess.Write))
            {
                BufferReader<TStream> buffer = new BufferReader<TStream>(_stream, 1024);

                while (buffer.RemainingBytes > 0)
                {
                    int readBytes = buffer.Next();
                    writer.Write(buffer.Value, 0, readBytes);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (File.Exists(_tmpPath))
                File.Delete(_tmpPath);

            Stream.Dispose();
        }

        private bool _disposed = false;
    }
}
