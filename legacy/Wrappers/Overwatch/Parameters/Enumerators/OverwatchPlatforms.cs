namespace Orikivo.Systems.Wrappers.Overwatch.Parameters.Enumerators
{
    public enum OverwatchPlatforms
    {
        Pc, Xbl, Psn
    }

    public static class PlatformReturn
    {
        public static string ToPlatformString(this OverwatchPlatforms p)
        {
            switch (p)
            {
                case OverwatchPlatforms.Pc:
                    return "pc";
                case OverwatchPlatforms.Psn:
                    return "psn";
                case OverwatchPlatforms.Xbl:
                    return "xbl";
                default:
                    return "pc";
            }
        }
    }
}