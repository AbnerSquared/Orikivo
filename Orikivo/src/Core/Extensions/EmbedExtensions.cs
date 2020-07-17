using Discord;
using Orikivo.Drawing;

namespace Orikivo
{
    public static class EmbedExtensions
    {
        public static EmbedBuilder WithColor(this EmbedBuilder eb, UserStatus status)
            => eb.WithColor(EmbedUtils.GetColorByStatus(status));

        public static EmbedBuilder WithColor(this EmbedBuilder e, System.Drawing.Color color)
            => e.WithColor(color.R, color.G, color.B);

        public static EmbedBuilder WithColor(this EmbedBuilder e, ImmutableColor color)
            => e.WithColor(color.R, color.G, color.B);

        public static EmbedBuilder WithFooter(this EmbedBuilder eb, int currentPage, int maxPage, string text = null, string iconUrl = null)
            => eb.WithFooter(EmbedUtils.CreatePageIndex(currentPage, maxPage, text), iconUrl);

        public static EmbedBuilder WithLocalImageUrl(this EmbedBuilder eb, string path)
            => eb.WithImageUrl(EmbedUtils.CreateLocalImageUrl(path));
    }
}
