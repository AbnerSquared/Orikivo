using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arcadia.Games
{
    public class CatanGame
    {
        public List<CatanPlayer> Players { get; set; }

        public CatanMap Map { get; set; }

        public CatanPlayer GetPlayer(CatanColorType color)
            => Players.First(x => x.Color == color);

        public int GetPointsFor(CatanColorType color)
        {
            int points = 0;

            foreach(CatanCity city in Map.Cities)
            {
                points += city.Tier; // 1 OR 2
            }

            foreach((CatanCardType type, int amount) in GetPlayer(color).Cards)
            {
                if (type == CatanCardType.Victory)
                    points += amount;
            }

            return points;
        }
    }
}
