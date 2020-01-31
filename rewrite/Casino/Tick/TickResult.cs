using Orikivo.Unstable;

namespace Orikivo.Casino
{
    public class TickResult : ICasinoResult
    {
        // The higher the risk
        // The higher the muliplier, but the lower chance to win
        public float Risk { get; }
        public TickWinMethod Method { get; }
        public int ExpectedTick { get; }
        public int ActualTick { get; }
        public long Wager { get; }
        public float Multiplier { get; }
        public long Reward { get; }
        public bool IsSuccess { get; }
        public Message ApplyAndDisplay(User user)
        {
            MessageBuilder builder = new MessageBuilder();
            Embedder embedder = new Embedder();

            return builder.Build();
        }
    }
}
