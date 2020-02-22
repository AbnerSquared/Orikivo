using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Orikivo
{
    public static class EnumUtils
    {
        public static List<TEnum> GetValues<TEnum>() where TEnum : Enum
            => typeof(TEnum).GetEnumValues().Cast<TEnum>().ToList();

        // gets all active flags on an enum.
        public static List<TEnum> GetFlags<TEnum>(TEnum value) where TEnum : Enum
        {
            List<TEnum> flags = new List<TEnum>();
            foreach(TEnum flag in GetValues<TEnum>())
            {
                if (value.HasFlag(flag))
                    flags.Add(flag);
            }

            return flags;
        }

        public static UrlType? GetUrlType(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;
            string ext = Path.GetExtension(url).Substring(1);
            if (ext.EqualsAny("png", "jpg", "gif"))
                return UrlType.Image;
            if (ext.EqualsAny("mp4", "mov"))
                return UrlType.Video;
            if (ext.EqualsAny("mp3", "wav"))
                return UrlType.Audio;
            if (ext.EqualsAny("txt", "cs", "js", "html", "cpp", "py"))
                return UrlType.Text;
            return UrlType.Empty;
        }
    }
}
