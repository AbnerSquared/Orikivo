using Discord;

namespace Orikivo
{
    /// <summary>
    /// Represents the content from an <see cref="IMessage"/>.
    /// </summary>
    public class MessageContent
    {
        public string Content { get; set; }
        
        public bool IsTTS { get; set; }

        public EmbedBuilder Embed { get; set; }
    }
}
