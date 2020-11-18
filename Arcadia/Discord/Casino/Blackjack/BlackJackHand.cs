using System.Collections.Generic;
using Orikivo;

namespace Arcadia.Casino
{
    public class BlackJackHand
    {
        public List<Card> Cards { get; internal set; } = new List<Card>();

        public long Wager { get; internal set; } = 0;

        public BlackJackState State { get; internal set; } = BlackJackState.Active;
    }
}
