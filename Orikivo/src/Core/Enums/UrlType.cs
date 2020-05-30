using System;

namespace Orikivo
{
    [Flags]
    public enum UrlType
    {
        Empty = 0, // no file type found at the end.
        Png = 1,
        Gif = 2,
        Jpeg = 4,
        Mp4 = 8,
        Mov = 16,
        Mp3 = 32,
        Wav = 64,
        Image = Png | Gif | Jpeg, // .png, .gif, .jpg
        Video = Mp4 | Mov, // .mp4, .mov
        Audio = Mp3 | Wav, // .mp3, .wav
        Text = 4 // .cs, .json, .txt, .html, .css
    }
}
