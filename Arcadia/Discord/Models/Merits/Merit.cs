using System;
using Arcadia.Models;

namespace Arcadia
{

    /// <summary>
    /// Represents an achievement.
    /// </summary>
    public class Merit : IModel<string>
    {
        public string Id { get; internal set; }

        public string Icon { get; internal set; }

        public string Name { get; internal set; }

        public string Quote { get; internal set; }

        public string LockQuote { get; internal set; }

        public MeritTag Tags { get; internal set; }

        public MeritRank Rank { get; internal set; }

        public long Score { get; internal set; }

        public bool Hidden { get; internal set; }

        public Func<ArcadeUser, bool> Criteria { get; internal set; } // public Dictionary<string, long> Criteria { get; internal set; }

        public Reward Reward { get; internal set; }
    }
}
