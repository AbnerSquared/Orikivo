using Discord;
using System.Text;

namespace Orikivo
{
    public class Message
    {
        public Message(MessageBuilder builder)
        {
            if (Check.NotNull(builder.Embedder))
            {
                Text = string.Empty;
                EmbedBuilder embed = new EmbedBuilder();
                embed.Description = builder.Content;

                if (builder.Embedder.Color.HasValue)
                    embed.WithColor(builder.Embedder.Color.Value);

                if (Check.NotNull(builder.Embedder.Footer))
                    embed.WithFooter(builder.Embedder.Footer);

                if (Check.NotNull(builder.Embedder.Header))
                    embed.WithTitle(builder.Embedder.Header);

                if (builder.HasUrl && builder.CanEmbedUrl)
                {
                    if (builder.HideUrl)
                        embed.Description += '\n' + OriFormat.Hyperlink(builder.Url);
                    else
                        embed.WithImageUrl(builder.IsLocalUrl ? EmbedUtils.CreateLocalImageUrl(builder.Url) : builder.Url);
                }
                Embed = embed.Build();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                if (Check.NotNull(builder.Content))
                    sb.AppendLine(builder.Content);
                if (builder.HasUrl)
                {
                    if (builder.HideUrl)
                        sb.AppendLine(Format.EscapeUrl(builder.Url));
                    else if (builder.IsLocalUrl)
                        AttachmentUrl = builder.Url;
                    else
                        sb.AppendLine(builder.Url);
                }

                Text = sb.ToString();
                IsTTS = builder.IsTTS;
                IsSpoiler = builder.IsSpoiler;

                
            }
        }

        public Message(ErrorMessageBuilder builder)
        {
            if (Check.NotNull(builder.Color))
            {
                EmbedBuilder embed = new EmbedBuilder();

                embed.WithColor(builder.Color);

                if (Check.NotNull(builder.Reaction))
                    embed.WithTitle(Format.Bold(builder.Reaction));

                embed.WithDescription(OriFormat.Error(builder.Reaction, builder.Title, builder.Reason, builder.StackTrace, Check.NotNull(builder.Color)));

                Embed = embed.Build();
            }
            else
            {
                Text = OriFormat.Error(builder.Reaction, builder.Title, builder.Reason, builder.StackTrace);
            }
        }

        public string Text { get; }
        public string AttachmentUrl { get; }
        public Embed Embed { get; }
        public bool IsTTS { get; }

        public bool IsSpoiler { get; }
    }
}
