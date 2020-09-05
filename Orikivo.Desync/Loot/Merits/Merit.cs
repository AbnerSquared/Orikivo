using Orikivo.Drawing;
using System;

namespace Orikivo.Desync
{
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

        public MeritGroup Group { get; set; } = MeritGroup.Misc;

        public MeritRank Rank { get; set; }

        // TODO: Implement explicit Criteria (UserCriteria)
        public Func<Husk, HuskBrain, bool> Criteria { get; set; }

        public Reward Reward { get; set; }

        public MeritData GetData()
            => new MeritData(DateTime.UtcNow, Reward != null ? false : (bool?) null);
    }
}
