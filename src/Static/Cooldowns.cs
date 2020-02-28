namespace Orikivo
{
    /// <summary>
    /// Contains a collection of cooldown identifiers.
    /// </summary>
    internal static class Cooldowns
    {
        internal static readonly string Global = "cooldown:global";
        internal static readonly string Notify = "cooldown:notify";
        internal static readonly string Vote = "cooldown:vote";
        internal static readonly string Daily = "cooldown:checked_in";
    }
}
