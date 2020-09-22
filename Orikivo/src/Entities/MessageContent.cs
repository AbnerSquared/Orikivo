using Discord;

namespace Orikivo
{
    /// <summary>
    /// Represents the content from an <see cref="IMessage"/>.
    /// </summary>
    public class MessageContent
    {
        public MessageContent(string content = null, bool isTTS = false, Embed embed = null)
        {
            Content = content;
            IsTTS = isTTS;
            Embed = embed?.ToEmbedBuilder();
        }

        public string Content { get; set; }

        public bool IsTTS { get; set; }

        public EmbedBuilder Embed { get; set; }
    }
}
