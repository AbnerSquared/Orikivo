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
                Text = "";
                var embed = new EmbedBuilder();
                embed.Description = builder.Content;

                if (builder.Embedder.Color.HasValue)
                    embed.WithColor(builder.Embedder.Color.Value);

                if (Check.NotNull(builder.Embedder.Footer))
                    embed.WithFooter(builder.Embedder.Footer);

                if (Check.NotNull(builder.Embedder.Header))
                    embed.WithTitle(builder.Embedder.Header);

                if (builder.HasUrl && builder.CanEmbedUrl)
                {
                    if (builder.Url.IsHidden)
                        embed.Description += '\n' + Format.Hyperlink(builder.Url);
                    else
                    {
                        embed.WithImageUrl(builder.Url.IsLocal
                            ? EmbedUtils.CreateLocalImageUrl(builder.Url)
                            : builder.Url.Value);

                        if (builder.Url.IsLocal)
                            AttachmentUrl = builder.Url;
                    }
                }
                Embed = embed.Build();
            }
            else
            {
                var text = new StringBuilder();

                if (Check.NotNull(builder.Content))
                    text.AppendLine(builder.Content);

                if (builder.HasUrl)
                {
                    if (builder.Url.IsHidden)
                        text.AppendLine(Format.EscapeUrl(builder.Url));
                    else if (builder.Url.IsLocal)
                        AttachmentUrl = builder.Url;
                    else
                        text.AppendLine(builder.Url);
                }

                Text = text.ToString();
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
                    embed.WithTitle(Discord.Format.Bold(builder.Reaction));

                embed.WithDescription(Format.Error(builder.Reaction, builder.Title, builder.Reason, builder.StackTrace, Check.NotNull(builder.Color)));

                Embed = embed.Build();
            }
            else
            {
                Text = Format.Error(builder.Reaction, builder.Title, builder.Reason, builder.StackTrace);
            }
        }

        public string Text { get; }
        public string AttachmentUrl { get; }
        public Embed Embed { get; }
        public bool IsTTS { get; }

        public bool IsSpoiler { get; }
    }
}
