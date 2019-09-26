namespace Orikivo.Systems.Wrappers.Overwatch.Parameters.Enumerators
{
    public enum OverwatchRegions
    {
        Us, Eu, Asia
    }

    public static class RegionReturn
    {
        public static string ToRegionString(this OverwatchRegions r)
        {
            switch (r)
            {
                case OverwatchRegions.Us:
                    return "us";
                case OverwatchRegions.Eu:
                    return "eu";
                case OverwatchRegions.Asia:
                    return "asia";
                default:
                    return "us";
            }
        }
    }
}