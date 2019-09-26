using Orikivo.Systems.Wrappers.Overwatch.Parameters.Enumerators;

namespace Orikivo.Systems.Wrappers.Overwatch.Parameters
{
    public class OverwatchProfile
    {
        public OverwatchPlatforms Platform { get; set; }
        public OverwatchRegions Region { get; set; }
        public string BattleTag { get; set; }
    }
}