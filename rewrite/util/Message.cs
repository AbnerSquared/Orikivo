using Discord;
using System.IO;
using System.Text;

namespace Orikivo
{
    public class Message
    {
        public Message(MessageBuilder builder)
        {
            if (Checks.NotNull(builder.Embedder))
            {
                Text = string.Empty;
                EmbedBuilder embed = new EmbedBuilder();
                embed.Description = builder.Content;
                embed.WithColor(builder.Embedder.Color ?? null);
                if (Checks.NotNull(builder.Embedder.Footer))
                    embed.WithFooter(builder.Embedder.Footer);
                if (Checks.NotNull(builder.Embedder.Header))
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
                if (Checks.NotNull(builder.Content))
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

                
            }
        }

        public Message(ErrorMessageBuilder builder)
        {
            if (Checks.NotNull(builder.Color))
            {
                EmbedBuilder embed = new EmbedBuilder();

                embed.WithColor(builder.Color);

                if (Checks.NotNull(builder.Reaction))
                    embed.WithTitle(Format.Bold(builder.Reaction));

                embed.WithDescription(OriFormat.Error(builder.Reaction, builder.Title, builder.Reason, builder.StackTrace, Checks.NotNull(builder.Color)));

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
    }
}
