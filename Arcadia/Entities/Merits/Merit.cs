using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using Orikivo;
using Orikivo.Desync;

namespace Arcadia
{
    // This is explicit SOLELY to be able to write criteria out.
    public class UserCriteria
    {
        public ulong ExpectedBalance { get; set; }
        public ulong ExpectedChipCount { get; set; }
        public ulong ExpectedTokenCount { get; set; }
        public ulong ExpectedDebt { get; set; }

        public int ExpectedAscent { get; set; }
        public ulong ExpectedExp { get; set; }

        public List<StatCriterion> ExpectedStats { get; set; }
    }

    // Utilized by multiple bot variants; this can be made a global class
    /// <summary>
    /// Represents an achievement for a <see cref="User"/>.
    /// </summary>
    public class Merit
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Sprite Icon { get; set; }

        public string Summary { get; set; }

        public string Quote { get; set; }

        public MeritGroup Group { get; set; } = MeritGroup.Generic;

        public MeritRank Rank { get; set; }

        public long Value { get; set; }

        public bool Hidden { get; set; }

        // TODO: Implement explicit Criteria (UserCriteria)
        public Func<ArcadeUser, bool> Criteria { get; set; }

        public Reward Reward { get; set; }

        public MeritData GetData()
            => new MeritData(DateTime.UtcNow, Check.NotNull(Reward) ? false : (bool?) null);
    }
}
