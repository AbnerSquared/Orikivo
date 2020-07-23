using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Orikivo
{
    public static class EnumUtils
    {
        public static List<TEnum> GetValues<TEnum>()
            where TEnum : Enum
            => typeof(TEnum).GetEnumValues().Cast<TEnum>().ToList();

        public static ExtensionType? GetUrlExtension(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            string ext = Path.GetExtension(url)[1..];

            if (ext.EqualsAny("png", "jpg", "gif"))
                return ExtensionType.Image;

            if (ext.EqualsAny("mp4", "mov"))
                return ExtensionType.Video;

            if (ext.EqualsAny("mp3", "wav"))
                return ExtensionType.Audio;

            if (ext.EqualsAny("txt", "cs", "js", "html", "cpp", "py"))
                return ExtensionType.Text;

            return ExtensionType.Empty;
        }
    }
}
