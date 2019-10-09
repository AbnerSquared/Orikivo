namespace Orikivo
{
    // the goal is to make game attributes that is required
    // be prebuilt in static classes, and load those.
    internal static class WolfAttributes
    {
        // keeps track of the current round.
        internal static string Round = "generic:round";
        internal static string Deaths = "generic:deaths";
        // Completes when its value is 1.
        internal static string ForceComplete = "generic:force_complete";
        // Completes when its value is 1.
        internal static string Call = "day:call";
        // Completes when its value is 3.
        internal static string FailedCalls = "day:failed_calls";
        // Completes when its value is 1.
        internal static string RemainSilent = "trial:remain_silent";
    }
}
