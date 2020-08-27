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
            => Players.Where(x => x.Color == color).First();

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

    public class CatanMap
    {
        public List<CatanLand> Lands { get; set; }

        public List<CatanPort> Ports { get; set; }

        public List<CatanCity> Cities { get; set; }

        public List<CatanRoad> Roads { get; set; }
    }

    public class CatanCity
    {
        // 1 for Settlement, 2 for City
        public int Tier { get; set; }

        public CatanColorType Color { get; set; }

        public CatanPoint Point { get; set; }
    }

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

    public enum CatanCardType
    {
        Knight = 1,
        Progress = 2,
        Victory = 3
    }

    public class CatanCard
    {
        public CatanCardType Type { get; set; }
    }

    public enum CatanColorType
    {
        White = 1,
        Red = 2,
        Blue = 3,
        Orange = 4,
        Green = 5,
        Brown = 6
    }

    public class CatanRoad
    {
        public CatanColorType Color { get; set; }

        public CatanPoint A { get; set; }
        
        public CatanPoint B { get; set; }


    }

    public class CatanPoint // this is inefficient, as there are connecting points to other hexes
    {
        public int HexRow { get; set; }
        public int HexColumn { get; set; }
        public char HexPoint { get; set; }
    

    
    }

    public class CatanLand
    {
        public int HexRow { get; set; }
        public int HexColumn { get; set; }
        public CatanLandType Type { get; set; }
    
        
        // If the land is a Desert, the token always == 0
        public int Token { get; set; } // 2 3 4 5 6 8 9 10 11 12
    
        // True if the robber is on this land.
        public bool HasRobber { get; set; }
    }

    public class CatanPort
    {
        public CatanResourceType Resource { get; set; }

        // Normally 3 if ANY
        // Otherwise, 2
        public int RequiredInput { get; set; }

        // The amount that is given
        // Normally, 1
        public int Output { get; set; }
    }

    [System.Flags]
    public enum CatanAward
    {
        None = 0,
        Army = 1,
        Road = 2,
        All = Army | Road
    }

    public enum CatanResourceType
    {
        Any = 0,
        Brick = 1,
        Lumber = 2,
        Ore = 3,
        Grain = 4,
        Wool = 5,
        None = 6
    }


    public enum CatanLandType
    {
        Hills = 1,
        Forest = 2,
        Mountains = 3,
        Fields = 4,
        Pasture = 5,
        Desert = 6
    }
}
