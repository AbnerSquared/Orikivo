using Discord;
using Discord.WebSocket;
using Orikivo.Systems.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Utility
{
    public static class Embedder
    {
        //public static List<EmbedPreset> Embeds { get; }
        //public static List<ColorPreset> Colors { get; }

        static Embedder()
        {
            //Embeds = new List<EmbedPreset>();
            //Colors = new List<ColorPreset>();
        }

        public static EmbedBuilder DefaultEmbed
        {
            get
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(EmbedData.GetColor("origreen"));
                return eb;
            }
        }

        //public static Color DefaultColor = Color.Blue;
        //public static Color ExceptionColor = Color.Red;
        public static EmbedBuilder ExceptionEmbed
        {
            get
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(EmbedData.GetColor("error"));
                return eb;
            }
        }

        // Incomplete segments.

        /*
        public static EmbedAuthorBuilder BuildAuthor(string name, string icon, string url)
        {
            EmbedAuthorBuilder ab = new EmbedAuthorBuilder();
            return ab;
        }
        public static EmbedFooterBuilder BuildFooter(string text, string icon)
        {
            EmbedFooterBuilder fb = new EmbedFooterBuilder();
            return fb;
        }
        public static EmbedFieldBuilder BuildField(string name, string value, bool inline)
        {
            EmbedAuthorBuilder ab = new EmbedAuthorBuilder();
            return ab;
        }
        public static EmbedFooterBuilder BuildFooter(string text, string icon)
        {
            EmbedFooterBuilder fb = new EmbedFooterBuilder();
            return fb;
        }
        public static EmbedFieldBuilder BuildField(string name, string value, bool inline)
        {
            EmbedFieldBuilder fib = new EmbedFieldBuilder();
            return fib;
        }*/
        //public static EmbedBuilder BuildEmbed(string title, string description, Color color, string imageurl, string thumbnailurl, DateTime timestamp)
        //public static EmbedBuilder AddFields(this EmbedBuilder eb, params (string name, string value)[] fields) {}
        //public static EmbedBuilder AddFields(this EmbedBuilder eb, params (string name, string value, bool inline)[] fields)
        //public static EmbedBuilder AddFields(this EmbedBuilder eb, params EmbedFieldBuilder[] fields)

        public static Embed Paginate(List<string> data, int page = 1, EmbedBuilder eb = null)
        {
            const int PER_PAGE = 20;
            int maxPages = (int)Math.Ceiling((double)(data.Count / PER_PAGE));
            page = page.InRange(1, maxPages);
            int pointer = (page - 1) * PER_PAGE;

            eb = eb ?? DefaultEmbed;
            int len = !string.IsNullOrWhiteSpace(eb.Description) ? eb.Description.Length : 0;

            // This way, the information written only stays on the first page.
            string description = pointer > 0 ? "" : eb.Description ?? "";

            // This shifts the title of an embed to the footer above the first page.
            string pager = $"{page}.{maxPages}";
            string footer = null;
            string title = null;
            //string footer = pointer > 0 ? eb.Title ?? "" : "";
            //string title = pointer > 0 ? "" : eb.Title ?? "";

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string s in data.Skip(pointer))
            {
                if (i >= PER_PAGE)
                    break;
                // + 1 is to account for the new line char.
                if ((len + sb.Length + s.Length) + 1 > EmbedBuilder.MaxDescriptionLength)
                    break;

                sb.AppendLine(s);
                i += 1;
            }

            sb.Insert(0, description);
            eb.WithDescription(sb.ToString());
            List<string> fb = new List<string>();

            if (eb.Footer.Exists())
                if (eb.Footer.Text.Exists())
                    fb.Add(eb.Footer.Text);

            if (footer.Exists())
                fb.Add(footer);

            if (maxPages > 1)
                fb.Add(pager);

            eb.WithFooter(fb.Conjoin(" | "));
            return eb.Build();
        }

        public static async Task PaginatedReplyAsync(this ISocketMessageChannel channel, List<string> data, int page = 1, EmbedBuilder eb = null)
        {
            await channel.SendEmbedAsync(Paginate(data, page, eb));
        }

        public static async Task SendEmbedAsync(this ISocketMessageChannel channel, Embed e)
        {
            await channel.SendMessageAsync(embed: e);
        }

        public static async Task CatchAsync(this ISocketMessageChannel channel, Exception exception)
        {
            EmbedBuilder eb = ExceptionEmbed;
            eb.WithTitle($"**Oops!**\nAn error has occured. `{exception.HResult}`");
            eb.WithDescription((exception.Message.Length > 2039 ? exception.Message.Substring(0, 2039) + "..." : exception.Message).DiscordBlock());
            eb.WithFooter(exception.StackTrace);
            await channel.SendEmbedAsync(eb.Build());
        }

        public static async Task SendSourceAsync(this ISocketMessageChannel channel, MessageStructure message)
        {
            if (!message.CanSend)
                throw new ArgumentException("The MessageStructure specified does not contains any valid fields that can send.");

            if (message.FilePath.Exists())
            {
                await channel.SendFileAsync(message.FilePath, message.Text, embed: message.Embed);
                return;
            }

            await channel.SendMessageAsync(message.Text, false, message.Embed);
        }

        public static async Task ThrowAsync(this ISocketMessageChannel channel, string error, string reason = null)
        {
            EmbedBuilder eb = ExceptionEmbed;
            eb.WithTitle(error);
            if (reason.Exists())
                eb.WithDescription(reason);
            await channel.SendEmbedAsync(eb.Build());
        }
    }
}