namespace Arcadia.Games
{
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
}