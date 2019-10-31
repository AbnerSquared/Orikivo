namespace Orikivo
{
    public struct CooldownInfo
    {
        public string Type { get; } // cooldown group name
        
        public double Seconds { get; } // cooldown length in seconds
        
        public bool CountStreaks { get; }
        
        public CooldownModuloInfo[] ModuloRewards { get; }
    }
}
