namespace Orikivo.Drawing.Encoding.Gif
{
    public class PlainTextExtension
    {
        byte Introducer;
        byte Label; // 01 == Plain Text Extension
        byte BlockSize;
        ushort LeftTextGrid; // x pos UInt16 MaxValue = 65535;
        ushort TopTextGrid; // y pos UInt16 MaxValue = 65535;
        ushort TextGridWidth;
        ushort TextGridHeight;
        byte CellWidth;
        byte CellHeight;
        byte TextForegroundColorIndex;
        byte TextBackgroundColorIndex;
        SubBlock[] PlainTextData; // infinite
        byte Terminator = 0x00;
    }
}
