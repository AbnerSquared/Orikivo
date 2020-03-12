using System;
using System.Drawing;
using System.IO;

namespace Orikivo
{
    // represents a group of blocks for a specified buffer size
    public class StreamBuffer<TStream> where TStream : Stream
    {
        public StreamBuffer(TStream stream, int bufferSize)
        {
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

        // reads the next buffer block
        public int Next()
        {
            if (RemainingBytes == 0)
                return 0;

            int readBytes = Stream.Read(Value, 0, Value.Length);
            ReadBytes += readBytes;

            return readBytes;
        }
    }

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
                StreamBuffer<TStream> buffer = new StreamBuffer<TStream>(_stream, 1024);

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

    /// <summary>
    /// Represents a configuration class that controls how the <see cref="Console"/> is displayed.
    /// </summary>
    public class ConsoleConfig
    {
        /// <summary>
        /// Returns the default <see cref="ConsoleConfig"/>.
        /// </summary>
        public static ConsoleConfig Default
        {
            get
            {
                return new ConsoleConfig
                {
                    BackgroundColor = ConsoleColor.DarkCyan,
                    TextColor = ConsoleColor.Cyan,
                    ShowCursor = false,
                    Position = null,
                    Size = null
                };
            }
        }

        /// <summary>
        /// Returns the current <see cref="ConsoleConfig"/> that is applied to the <see cref="Console"/>.
        /// </summary>
        public static ConsoleConfig Current
        {
            get
            {
                ConsoleConfig config = new ConsoleConfig();
                config.Synchronize();
                return config;
            }
        }

        /// <summary>
        /// Gets or sets the title of the <see cref="Console"/> window to use.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets an optional <see cref="ConsoleColor"/> value for the <see cref="Console.BackgroundColor"/>.
        /// </summary>
        public ConsoleColor? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets an optional <see cref="ConsoleColor"/> value for the <see cref="Console.ForegroundColor"/>.
        /// </summary>
        public ConsoleColor? TextColor { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> that determines if the <see cref="Console"/> should display the cursor.
        /// </summary>
        public bool ShowCursor { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Point"/> that represents the top-left position of the <see cref="Console"/> window.
        /// </summary>
        public Point? Position { get; set; }

        /// <summary>
        /// Gets or sets the position of the cursor for the <see cref="Console"/>.
        /// </summary>
        public Point? Cursor { get; set; }

        /// <summary>
        /// Gets or sets the size of the cursor for the <see cref="Console"/>.
        /// </summary>
        public int? CursorSize { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="System.Drawing.Size"/> that states the width and height of the <see cref="Console"/>.
        /// </summary>
        public Size? Size { get; set; }

        // TODO: Determine if this version can be set to reduce dependencies. The other versions are used to ensure that both are defined.
        //public int? X { get; set; }
        //public int? Y { get; set; }
        //public int? Width { get; set; }
        //public int? Height { get; set; }

        /// <summary>
        /// Updates the <see cref="ConsoleConfig"/> to represent the current <see cref="Console"/> configuration.
        /// </summary>
        public void Synchronize()
        {
            Position = new Point(Console.WindowLeft, Console.WindowTop);
            Size = new Size(Console.WindowWidth, Console.WindowWidth);
            Title = Console.Title;
            BackgroundColor = Console.BackgroundColor;
            TextColor = Console.ForegroundColor;
            ShowCursor = Console.CursorVisible;
            Cursor = new Point(Console.CursorLeft, Console.CursorTop);
            CursorSize = Console.CursorSize;
        }

        /// <summary>
        /// Applies the <see cref="ConsoleConfig"/> to the <see cref="Console"/>.
        /// </summary>
        public void Apply()
        {
            if (Position.HasValue)
                Console.SetWindowPosition(Position.Value.X, Position.Value.Y);

            if (Size.HasValue)
                Console.SetWindowSize(Size.Value.Width, Size.Value.Height);
            
            if (Check.NotNull(Title))
                Console.Title = Title;

            if (BackgroundColor.HasValue)
                Console.BackgroundColor = BackgroundColor.Value;

            if (TextColor.HasValue)
                Console.ForegroundColor = TextColor.Value;

            Console.CursorVisible = ShowCursor;

            if (Cursor.HasValue)
                Console.SetCursorPosition(Cursor.Value.X, Cursor.Value.Y);

            if (CursorSize.HasValue)
                Console.CursorSize = CursorSize.Value;
        }
    }
}
