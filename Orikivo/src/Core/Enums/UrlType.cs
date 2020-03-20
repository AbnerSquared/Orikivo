namespace Orikivo
{
    public enum UrlType
    {
        Empty = 0, // no file type found at the end.
        Image = 1, // .png, .gif, .jpg
        Video = 2, // .mp4, .mov
        Audio = 3, // .mp3, .wav
        Text = 4, // .cs, .json, .txt, .html, .css
    }
}
