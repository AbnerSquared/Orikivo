using Discord;
using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public static class EmbedUtils
    {
        /// <summary>
        /// Gets the color object derived from the status of a Discord user.
        /// </summary>
        public static System.Drawing.Color GetColorByStatus(UserStatus status)
        {
            return status.EqualsAny(UserStatus.Offline, UserStatus.Invisible) ? new GammaColor(0x747F8D)
                : status == UserStatus.DoNotDisturb ? new GammaColor(0xF04747)
                : status.EqualsAny(UserStatus.AFK, UserStatus.Idle) ? new GammaColor(0xFAA61A)
                : status == UserStatus.Online ? new GammaColor(0x43B581)
                : throw new Exception("The UserStatus given is unspecified.");
        }

        public static GammaColor GetColorByReport(int reportCount)
            => reportCount >= InfoService.CRITICAL_THRESHOLD ?
            new GammaColor(0xE75A70) : reportCount >= InfoService.YIELD_THRESHOLD ?
            new GammaColor(0xFFAC33) : new GammaColor(0x55ACEE);

        // TODO: actually give this a reason to exist.
        public static string CreatePageIndex(int currentPage, int maxPage, string text = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(currentPage);
            sb.Append(" of ");
            sb.Append(maxPage);
            if (Check.NotNull(text))
                sb.Insert(0, $"{text} | ");
            return sb.ToString();
        }

        public static string PaginateElementsAt(List<string> values, int page = 1)
        {
            const int MAX_VALUES_PER_PAGE = 20;
            StringBuilder sb = new StringBuilder();
            int pages = (int)Math.Ceiling((double)(values.Count / MAX_VALUES_PER_PAGE));
            page = page > pages ? pages : page < 1 ? 1 : page;

            foreach (string value in values.Skip(MAX_VALUES_PER_PAGE * (page - 1)))
                sb.AppendLine(value);

            return sb.ToString();
        }

        public static string CreateLocalImageUrl(string path)
            => $"attachment://{Path.GetFileName(path)}";
    }
}
