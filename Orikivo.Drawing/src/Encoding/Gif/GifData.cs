namespace Orikivo.Drawing.Encoding.Gif
{
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
        Header Header { get; set; }
        ScreenDescriptor LogicalScreenDescriptor { get; set; }
        ImageBlock[] Images { get; set; }

        byte Trailer;
    }
}
