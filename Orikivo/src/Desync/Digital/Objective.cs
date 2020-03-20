using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class Objective // GuildObjective : IObjective
    {
        // how hard an objective is.
        public int Rank { get; }
        public IReadOnlyList<ObjectiveCriterion> Criteria { get; }
        public Dictionary<string, int> Trackers { get; }
    }
}
