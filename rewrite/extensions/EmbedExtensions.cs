using Discord;
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

        public static EmbedBuilder WithFooter(this EmbedBuilder eb, int currentPage, int maxPage, string text = null, string iconUrl = null)
            => eb.WithFooter(EmbedUtils.CreatePagedFooter(currentPage, maxPage, text), iconUrl);

        // this must be sent using Context.Channel.SendFileAsync;
        public static EmbedBuilder WithAttachedImage(this EmbedBuilder eb, string path)
            => eb.WithImageUrl($"attachment://{Path.GetFileName(path)}");
    }
}
