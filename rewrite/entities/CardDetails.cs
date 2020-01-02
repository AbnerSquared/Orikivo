using Discord;
using Discord.WebSocket;
using Orikivo.Unstable;

namespace Orikivo
{
    /// <summary>
    /// Represents the details of a card.
    /// </summary>
    public class CardDetails
    {
        internal CardDetails() { }
        public CardDetails(User user, SocketUser socket)
        {
            Program = socket.Activity?.Name;
            Activity = socket.Activity?.Type;
            Ascent = 0;
            Status = socket.Status;
            Name = user.Username;
            AvatarUrl = socket.GetAvatarUrl(ImageFormat.Png, 32);
            Exp = user.Exp;
        }

        /// <summary>
        /// The user's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A Url that points to the user's avatar. If empty, it will default to generic avatar icon.
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// The user's current balance.
        /// </summary>
        public ulong Balance { get; set; }

        /// <summary>
        /// The user's current debt.
        /// </summary>
        public ulong Debt { get; set; }

        /// <summary>
        /// The user's current experience.
        /// </summary>
        public ulong Exp { get; set; }

        /// <summary>
        /// The user's current ascent, otherwise known as their rebirths.
        /// </summary>
        public int Ascent { get; set; }

        /// <summary>
        /// The user's current activity, if any.
        /// </summary>
        public string Program { get; set; }

        /// <summary>
        /// The current type of activity that the user is currently doing.
        /// </summary>
        public ActivityType? Activity { get; set; }

        public UserStatus Status { get; set; }
    }

    /// <summary>
    /// Represents the configurations that can be applied to the design of a card.
    /// </summary>
    public class CardConfig
    {
        /// <summary>
        /// The <see cref="FontFace"/> that is used when drawing the card.
        /// </summary>
        public string NameFontId { get; set; }
        public string CounterFontId { get; set; } // the font used when drawing counters

        public CardAvatarScale AvatarScale { get; set; }


    }

    public enum CardAvatarScale
    {
        Small = 16,
        Large = 32
    }
}
