using System.Drawing.Imaging;
using DiscordImageFormat = Discord.ImageFormat;
using SystemImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Orikivo
{
    public static class ImageFormatExtension
    {
        public static SystemImageFormat ToSystemFormat(this DiscordImageFormat f)
        {
            switch (f)
            {
                case DiscordImageFormat.Gif:
                    return SystemImageFormat.Gif;
                case DiscordImageFormat.Jpeg:
                    return SystemImageFormat.Jpeg;
                default:
                    return SystemImageFormat.Png;
            }
        }

        public static string GetExtensionName(this DiscordImageFormat f)
            => f.ToSystemFormat().GetExtensionName();

        public static string GetExtensionName(this SystemImageFormat f)
            => $".{f}".ToLower();

        public static ImageCodecInfo GetCodecInfo(this SystemImageFormat f)
            => BitmapManager.GetCodec(f);
    }
}