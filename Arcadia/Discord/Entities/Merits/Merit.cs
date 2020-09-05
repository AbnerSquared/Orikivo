using System;
using Orikivo;
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
        // The quote to display if the merit is currently locked
        public string LockQuote { get; set; }

        public MeritGroup Group { get; set; } = MeritGroup.Common;

        public MeritRank Rank { get; set; }

        public long Value { get; set; }

        public bool Hidden { get; set; }

        public Func<ArcadeUser, bool> Criteria { get; set; }

        public Reward Reward { get; set; }

        public MeritData GetData()
            => new MeritData(DateTime.UtcNow, Check.NotNull(Reward) ? false : (bool?) null);
    }
}
