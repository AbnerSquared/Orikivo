using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // a class that stores a pre-built message that can be sent to discord.
    /// <summary>
    /// Represents a custom message body that can be freely edited.
    /// </summary>
    public class OriMessage
    {
        public OriMessage() { }
        public OriMessage(string text, string url, OriEmbedOptions embedOptions)
        {
            Text = text;
            Url = url;
            EmbedOptions = embedOptions;
            IsTTS = false;
        }

        private EmbedBuilder _embed;
        public EmbedBuilder Embed
        {
            get
            {
                if (EmbedOptions != null)
                {
                    if (_embed != null)
                    {
                        // OriColor this up
                        _embed.Color = new Color(EmbedOptions.Color.r, EmbedOptions.Color.g, EmbedOptions.Color.b);
                        if (EmbedOptions.IsLocalImage && _embed.ImageUrl != null)
                            _embed.ImageUrl = $"attachment://{_embed.ImageUrl}";
                    }
                    else
                    {
                        _embed = new EmbedBuilder();
                        _embed.Color = new Color(EmbedOptions.Color.r, EmbedOptions.Color.g, EmbedOptions.Color.b);
                        if (UrlType == Orikivo.UrlType.Image)
                            _embed.ImageUrl = Url;
                    }
                }

                return _embed;
            }

            set
            {
                _embed = value;
            }
        }
        public OriEmbedOptions EmbedOptions { get; }
        public string Text { get; set; }
        public string Url { get; set; } // if there is a url.
        public bool HasUrl => !string.IsNullOrWhiteSpace(Url);
        public UrlType? UrlType => HasUrl ? CustomCommandMessage.GetUrlType(Url) : null;
        public bool IsTTS { get; set; }
    }
}
