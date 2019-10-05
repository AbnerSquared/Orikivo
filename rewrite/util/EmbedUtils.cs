using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public static class EmbedUtils
    {
        public static OriColor GetColorByStatus(UserStatus status)
        {
            return status.EqualsAny(UserStatus.Offline, UserStatus.Invisible) ? new OriColor(0x747F8D)
                : status == UserStatus.DoNotDisturb ? new OriColor(0xF04747)
                : status.EqualsAny(UserStatus.AFK, UserStatus.Idle) ? new OriColor(0xFAA61A)
                : status == UserStatus.Online ? new OriColor(0x43B581)
                : throw new Exception("The UserStatus given is unspecified.");
        }

        public static string CreatePagedFooter(int currentPage, int maxPage, string text = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(currentPage);
            sb.Append("...");
            sb.Append(maxPage);
            if (!string.IsNullOrWhiteSpace(text))
                sb.Insert(0, $"{text} | ");
            return sb.ToString();
        }
    }
}
