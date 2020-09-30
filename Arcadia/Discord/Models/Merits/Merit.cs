using System;
using Orikivo.Drawing;

namespace Arcadia
{
    /// <summary>
    /// Represents an achievement.
    /// </summary>
    public class Merit
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public Sprite Image { get; set; }
        public string Quote { get; set; }
        public string LockQuote { get; set; }
        public MeritTag Tag { get; set; }
        public MeritRank Rank { get; set; }
        public long Score { get; set; }
        public bool Hidden { get; set; }
        public Func<ArcadeUser, bool> Criteria { get; set; }
        public Reward Reward { get; set; }
    }
}
