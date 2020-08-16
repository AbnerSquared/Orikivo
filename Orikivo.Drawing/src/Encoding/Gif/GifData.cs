using System;
using System.IO;
using System.Linq;

namespace Orikivo.Drawing.Encoding.Gif
{
    internal static class StreamExtensions
    {
        public static void WriteInt32Byte(this Stream stream, int value)
        {
            stream.WriteByte(Convert.ToByte(value));
        }
        public static void WriteUInt16(this Stream stream, int value)
        {
            stream.WriteByte(Convert.ToByte(value & 0xFF));
            stream.WriteByte(Convert.ToByte((value >> 8) & 0xFF));
        }

        public static void WriteString(this Stream stream, string value)
        {
            stream.Write(value.Select(c => (byte)c).ToArray(), 0, value.Length);
        }
    }
    /*

        public static byte[] ToBytes(this ushort value)
        {
            var bytes = new byte[2];

            bytes[0] = (byte) (value & 0xFF);
            bytes[1] = (byte) ((value >> 8) & 0xFF);

            return bytes;
        }
     
        Block Name                  Required   Label       Ext.   Vers.
        Application Extension       Opt. (*)   0xFF (255)  yes    89a
        Comment Extension           Opt. (*)   0xFE (254)  yes    89a
        Global Color Table          Opt. (1)   none        no     87a
        Graphic Control Extension   Opt. (*)   0xF9 (249)  yes    89a
        Header                      Req. (1)   none        no     N/A
        Image Descriptor            Opt. (*)   0x2C (044)  no     87a (89a)
        Local Color Table           Opt. (*)   none        no     87a
        Logical Screen Descriptor   Req. (1)   none        no     87a (89a)
        Plain Text Extension        Opt. (*)   0x01 (001)  yes    89a
        Trailer                     Req. (1)   0x3B (059)  no     87a
        
        Unlabeled Blocks
        Header                      Req. (1)   none        no     N/A
        Logical Screen Descriptor   Req. (1)   none        no     87a (89a)
        Global Color Table          Opt. (1)   none        no     87a
        Local Color Table           Opt. (*)   none        no     87a
        
        Graphic-Rendering Blocks
        Plain Text Extension        Opt. (*)   0x01 (001)  yes    89a
        Image Descriptor            Opt. (*)   0x2C (044)  no     87a (89a)
        
        Control Blocks
        Graphic Control Extension   Opt. (*)   0xF9 (249)  yes    89a
        
        Special Purpose Blocks
        Trailer                     Req. (1)   0x3B (059)  no     87a
        Comment Extension           Opt. (*)   0xFE (254)  yes    89a
        Application Extension       Opt. (*)   0xFF (255)  yes    89a
        
        legend:           (1)   if present, at most one occurrence
                          (*)   zero or more occurrences
                          (+)   one or more occurrences 
    */
    /*
       FILE_TYPE = "GIF";
       FILE_VERSION = "89a"; // or 87a
       FILE_TRAILER = 0x3B; // ;
       APP_BLOCK_ID = 0xFF21; // FF + 21 = FF + '!'
       APP_BLOCK_SIZE = 0x0B;
       APP_ID = "NETSCAPE2.0";
       GRAPHIC_CONTROL_BLOCK_ID = 0xF921; // F9 + 21 = F9 + '!'
       GRAPHIC_CONTROL_BLOCK_SIZE = 0x04;
       GLOBAL_COLOR_TABLE_POS = 10;

       info about where the Graphics Control is located
       GRAPHIC_CONTROL_POS = 781;
       GRAPHIC_CONTROL_LENGTH = 8;

        the image block location
       IMAGE_BLOCK_POS = 789;
       IMAGE_BLOCK_HEADER_LENGTH = 11;
       COLOR_BLOCK_POS = 13;
       COLOR_BLOCK_LENGTH = 768; // 256 entries / 3
      
        GIF87a Layout
        - Header and Color Table Information
            - Header
            - Logical Screen Descriptor
            - Global Color Table
        - Images[]
            - Local Image Descriptor
            - Local Color Table
            - Image Data
        - Trailer
     
        
        GIF89a Layout
        - Header and Color Table Information
            - Header
            - Logical Screen Descriptor
            - Global Color Table
        - Extension Information
            - Comment Extension
            - Application Extension
            - Graphic Control Extension
        - Images[]
            - Local Image Descriptor
            - Local Color Table
            - Image Data, each row length is specified, along with argb bytes
        - Extension Information
            - Comment Extension
            - Plain Text Extension
        - Trailer     
     */

    public enum Version
    {
        GIF87a,
        GIF89a
    }

    // if GifVersion is Gif87a, it CANNOT support extensions
    public class GifData
    {
        // This is used to mark the end of a GIF data stream. This should always be the final byte
        private const byte TRAILER = 0x3B;

        public Header Header;
        public ScreenDescriptor LogicalScreenDescriptor;

        public ColorTriplet[] GlobalColorTable;
        public ImageBlock[] Images;
    }

    public class GifDecoder
    {

    }
}
