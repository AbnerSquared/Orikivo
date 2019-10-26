using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Represents a custom message body that can be freely edited.
    /// </summary>
    public class MessageBuilder
    {
        public MessageBuilder() { }
        public MessageBuilder(string content, string url)
        {
            Content = content;
            Url = url;
            IsTTS = false;
        }

        public Embedder Embedder { get; set; } // If null, write as a message.
        public string Content { get; set; }
        public string Url { get; set; } // if there is a url.
        public bool HasUrl => Checks.NotNull(Url);
        public bool HideUrl { get; set; } = false; // if the url is hyperlinked.
        public bool IsLocalUrl { get; set; } = false;
        public bool CanEmbedUrl => Checks.NotNull(Embedder) && CustomCommandMessage.GetUrlType(Url).Value == UrlType.Image;
        public UrlType? FileType => HasUrl ? CustomCommandMessage.GetUrlType(Url) : null;
        public bool IsTTS { get; set; }

        public Message Build()
            => new Message(this);
    }
}
