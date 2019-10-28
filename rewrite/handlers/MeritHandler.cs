using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{
    /// <summary>
    /// Handles when merits are awarded, alongside determining what a merit is.
    /// </summary>
    public class MeritHandler
    {
        public MeritHandler() => throw new NotImplementedException();

        public List<IMerit> Merits { get; }

        public async Task CheckUserAsync(OriUser user)
        {
            // check if the user's new stat data met any merit criteria.
        }
        // check criteria
        // catch Event.StatsUpdated
        // check if the stats updated now fit the criteria of a merit
        // check rewarding
    }
}
