namespace Orikivo
{
    /// <summary>
    /// A small packet of information defining a vote.
    /// </summary>
    public class VoteInfo
    {
        internal VoteInfo(ulong userId, VoteType vote = VoteType.Upvote)
        {
            UserId = userId;
            Vote = vote;
        }

        /// <summary>
        /// The Discord Snowflake ID of the user that voted.
        /// </summary>
        public ulong UserId { get; }

        /// <summary>
        /// The type of vote that the user used.
        /// </summary>
        public VoteType Vote { get; internal set; }
    }
}
