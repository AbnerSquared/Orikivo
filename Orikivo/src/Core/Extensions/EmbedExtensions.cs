using Discord;
using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Orikivo
{
    public static class EmbedExtensions
    {
        public static EmbedBuilder WithColor(this EmbedBuilder eb, UserStatus status)
            => eb.WithColor(EmbedUtils.GetColorByStatus(status));

        public static EmbedBuilder WithColor(this EmbedBuilder e, System.Drawing.Color color)
            => e.WithColor(color.R, color.G, color.B);

        public static EmbedBuilder WithColor(this EmbedBuilder e, GammaColor color)
            => e.WithColor(color.R, color.G, color.B);

        public static EmbedBuilder WithFooter(this EmbedBuilder eb, int currentPage, int maxPage, string text = null, string iconUrl = null)
            => eb.WithFooter(EmbedUtils.CreatePageIndex(currentPage, maxPage, text), iconUrl);

        // this must be sent using Context.Channel.SendFileAsync;
        public static EmbedBuilder WithLocalImageUrl(this EmbedBuilder eb, string path)
            => eb.WithImageUrl(EmbedUtils.CreateLocalImageUrl(path));
    }
}
