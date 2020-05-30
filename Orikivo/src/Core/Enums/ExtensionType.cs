using System;

namespace Orikivo
{
    [Flags]
    public enum ExtensionType
    {
        // no extension was specified
        Empty = -1,

        // as long as an extension is specified, this passes
        Any = 0,

        Txt = 1,
        Json = 2,
        
        Png = 4,
        Gif = 8,
        Jpeg = 16,
        
        Mp4 = 32,
        Mov = 64,
        
        Mp3 = 128,
        Wav = 256,

        Text = Txt | Json,
        Image = Png | Gif | Jpeg,
        Video = Mp4 | Mov,
        Audio = Mp3 | Wav
    }
}
