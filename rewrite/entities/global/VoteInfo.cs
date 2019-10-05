namespace Orikivo
{
    public class VoteInfo
    {
        internal VoteInfo(ulong userId, VoteType vote = VoteType.Upvote)
        {
            UserId = userId;
            Vote = vote;
        }
        public ulong UserId { get; }
        public VoteType Vote { get; internal set; }
    }
}
