using Discord;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Represents the details of a card.
    /// </summary>
    public class CardDetails
    {
        internal CardDetails() { }
        public CardDetails(ArcadeUser user, IUser socket)
        {
            Program = socket.Activity?.Name;
            Activity = socket.Activity?.Type;
            Ascent = user.Ascent;
            Status = socket.Status;
            Name = user.Username;
            Balance = user.Balance;
            Debt = user.Debt;
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
        public long Balance { get; set; }

        /// <summary>
        /// The user's current debt.
        /// </summary>
        public long Debt { get; set; }

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
}
