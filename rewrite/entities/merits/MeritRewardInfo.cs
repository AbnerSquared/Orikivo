using System.Collections.Generic;

namespace Orikivo
{
    // Used to determine what is given to a user upon claiming this merit.
    public class MeritRewardInfo
    {
        /// <summary>
        /// Defines the ID of an item alongside its count.
        /// </summary>
        public Dictionary<string, int> ItemIds { get; }
        public ulong? Balance { get; }
    }
}
