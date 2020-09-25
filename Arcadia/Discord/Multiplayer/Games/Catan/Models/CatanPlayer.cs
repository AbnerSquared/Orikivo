using System.Collections.Generic;

namespace Arcadia.Games
{
    public class CatanPlayer
    {
        // The color of the player
        public CatanColorType Color { get; set; }

        // The collection of resources that this player has.
        public Dictionary<CatanResourceType, int> Resources { get; set; }

        // The collection of development cards this player has.
        public Dictionary<CatanCardType, int> Cards { get; set; }

        public CatanAward Awards { get; set; }
    }
}