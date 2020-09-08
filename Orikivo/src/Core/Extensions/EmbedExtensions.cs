using Discord;
using Orikivo.Drawing;

namespace Orikivo
{
    public static class EmbedExtensions
    {
        /// <summary>
        /// Adds embed color based on the provided <see cref="UserStatus"/>.
        /// </summary>
        public static EmbedBuilder WithColor(this EmbedBuilder eb, UserStatus status)
            => eb.WithColor(EmbedUtils.GetColorByStatus(status));

        /// <summary>
        /// Adds embed color based on the provided <see cref="System.Drawing.Color"/> value.
        /// </summary>
        public static EmbedBuilder WithColor(this EmbedBuilder e, System.Drawing.Color color)
            => e.WithColor(color.R, color.G, color.B);

        /// <summary>
        /// Adds embed color based on the provided <see cref="ImmutableColor"/> value.
        /// </summary>
        public static EmbedBuilder WithColor(this EmbedBuilder e, ImmutableColor color)
            => e.WithColor(color.R, color.G, color.B);

        /// <summary>
        /// Sets the image URL of an <see cref="Embed"/> as an attachment reference.
        /// </summary>
        public static EmbedBuilder WithLocalImageUrl(this EmbedBuilder eb, string path)
            => eb.WithImageUrl(EmbedUtils.CreateLocalImageUrl(path));
    }
}
