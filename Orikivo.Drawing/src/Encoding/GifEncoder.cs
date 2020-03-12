using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Orikivo.Drawing.Encoding
{

    // TODO: Create a Decoder version of this class, which allows for the loading of GIF files, frame-by-frame.
    // This could most likely be utilized with Encoding/Raw, as the base classes contain what a GIF needs to decode.

    // NOTE: Referenced from the following GitHub projects:
    // https://github.com/DataDink/Bumpkit/blob/master/BumpKit/BumpKit/GifEncoder.cs
    /// <summary>
    /// A disposable class providing access to construct Graphical Interchange Format files, otherwise known as GIF.
    /// </summary>
    public class GifEncoder : IDisposable
    {
        private const string FILE_TYPE = "GIF";
        private const string FILE_VERSION = "89a"; // or 87a
        private const byte FILE_TRAILER = 0x3B; // ;
        private const int APP_BLOCK_ID = 0xFF21; // FF + 21 = FF + '!'
        private const byte APP_BLOCK_SIZE = 0x0B;
        private const string APP_ID = "NETSCAPE2.0";
        private const int GRAPHIC_CONTROL_BLOCK_ID = 0xF921; // F9 + 21 = F9 + '!'
        private const byte GRAPHIC_CONTROL_BLOCK_SIZE = 0x04;
        private const long GLOBAL_COLOR_TABLE_POS = 10;
        private const long GRAPHIC_CONTROL_POS = 781;
        private const long GRAPHIC_CONTROL_LENGTH = 8;
        private const long IMAGE_BLOCK_POS = 789;
        private const long IMAGE_BLOCK_HEADER_LENGTH = 11;
        private const long COLOR_BLOCK_POS = 13;
        private const long COLOR_BLOCK_LENGTH = 768; // 256 entries / 3

        private bool _isFirstImage = true;
        private int? _width;
        private int? _height;
        private int? _repeatCount;
        private readonly Stream _stream;

        public TimeSpan FrameLength { get; set; }
        public Quality Quality { get; set; } = Quality.Bpp8;

        public GifEncoder(Stream stream, Size size, int? repeatCount = null)
        {
            _stream = stream;
            _width = size.Width;
            _height = size.Height;
            _repeatCount = repeatCount;
        }

        public GifEncoder(Stream stream, int? width = null, int? height = null, int? repeatCount = null)
        {
            Console.WriteLine("GIF encoder opened.");
            _stream = stream;
            _width = width;
            _height = height;
            _repeatCount = repeatCount;
        }

        public void EncodeFrame(Image image, Point? offset = null, TimeSpan? frameLength = null)
        {
            if (image.Width > ushort.MaxValue || image.Height > ushort.MaxValue)
                throw new ArgumentException("The image specified cannot have a width or height of 65535.");

            Point imageOffset = offset ?? Point.Empty;
            // TODO: Handle out of bounds offset. (Refer to DrawableLayer.Build())

            using (MemoryStream source = new MemoryStream())
            {
                // This makes sure that the color palettes for the GIF are neatly stored.
                EncodeUtils.CreateGifStream(image, source, Quality); 
                if (_isFirstImage)
                {
                    WriteHeader(source, image.Width, image.Height);
                }

                WriteGraphicControlBlock(source, frameLength.GetValueOrDefault(FrameLength));
                WriteImageBlock(source, !_isFirstImage, imageOffset.X, imageOffset.Y, image.Width, image.Height);
            }

            _isFirstImage = false;
        }

        private void WriteHeader(Stream source, int width, int height)
        {
            WriteString(FILE_TYPE);
            WriteString(FILE_VERSION);
            WriteShort(_width.GetValueOrDefault(width));
            WriteShort(_height.GetValueOrDefault(height));
            source.Position = GLOBAL_COLOR_TABLE_POS;
            WriteByte(source.ReadByte());
            WriteByte(0); // BG Color Index
            WriteByte(0); // PIXEL ASPECT RATIO
            WriteColorTable(source);
            WriteShort(APP_BLOCK_ID);
            WriteByte(APP_BLOCK_SIZE);
            WriteString(APP_ID);
            WriteByte(3);
            WriteByte(1);
            WriteShort(_repeatCount.GetValueOrDefault(0));
            WriteByte(0); // TERMINATOR: 0x00
        }

        // get colors from stream.
        private void WriteColorTable(Stream source)
        {
            source.Position = COLOR_BLOCK_POS;
            byte[] colorTable = new byte[COLOR_BLOCK_LENGTH]; // 256 colors, RGB
            source.Read(colorTable, 0, colorTable.Length); // gets all bytes in this area.
            _stream.Write(colorTable, 0, colorTable.Length); // writes all read bytes to image
        }

        private void WriteGraphicControlBlock(Stream source, TimeSpan frameDelay)
        {
            source.Position = GRAPHIC_CONTROL_POS;
            byte[] graphicBlock = new byte[GRAPHIC_CONTROL_LENGTH];
            source.Read(graphicBlock, 0, graphicBlock.Length);

            WriteShort(GRAPHIC_CONTROL_BLOCK_ID);
            WriteByte(GRAPHIC_CONTROL_BLOCK_SIZE);
            WriteByte(graphicBlock[3] & 0xF7 | 0x08); // Disposal flag
            WriteShort(Convert.ToInt32(frameDelay.TotalMilliseconds / 10)); // Frame delay
            WriteByte(graphicBlock[6]); // transparent color index
            WriteByte(0); // Terminator
        }

        private void WriteImageBlock(Stream source, bool includeColorTable, int x, int y, int height, int width)
        {
            source.Position = IMAGE_BLOCK_POS;
            byte[] header = new byte[IMAGE_BLOCK_HEADER_LENGTH];
            source.Read(header, 0, header.Length);
            WriteByte(header[0]);
            WriteShort(x); // x pos
            WriteShort(y); // y pos
            WriteShort(height); // Height
            WriteShort(width); // Width
        
            if (includeColorTable)
            {
                source.Position = GLOBAL_COLOR_TABLE_POS;
                WriteByte(source.ReadByte() & 0x3F | 0x80); // enable local color table
                WriteColorTable(source);
            }
            else
            {
                WriteByte(header[9] & 0x07 | 0x07); // disable local color table
            }

            WriteByte(header[10]); // LZW min code size

            source.Position = IMAGE_BLOCK_POS + IMAGE_BLOCK_HEADER_LENGTH;

            int dataLength = source.ReadByte(); // i assume this is a row length
            while (dataLength > 0)
            {
                byte[] imageData = new byte[dataLength];
                source.Read(imageData, 0, dataLength);

                WriteByte(dataLength);
                _stream.Write(imageData, 0, dataLength);
                dataLength = source.ReadByte(); // row length descriptor
            }

            _stream.WriteByte(0); // TERMINATOR;

        }

        private void WriteByte(int value)
        {
            _stream.WriteByte(Convert.ToByte(value));
        }

        private void WriteShort(int value)
        {
            _stream.WriteByte(Convert.ToByte(value & 0xFF));
            _stream.WriteByte(Convert.ToByte((value >> 8) & 0xFF)); // shift value right 8
        }

        private void WriteString(string value)
        {
            _stream.Write(value.ToArray().Select(c => (byte)c).ToArray(), 0, value.Length);   
        }

        public void Dispose()
        {
            WriteByte(FILE_TRAILER); // closes
            _stream.Flush(); // pushes data
            Console.WriteLine("GIF encoded.");

            _stream.Position = 0;
        }
    }
}
