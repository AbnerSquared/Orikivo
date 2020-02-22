namespace Orikivo
{
    // TODO: Create AttributeReader, which can easily account for all of these.
    internal static class Cooldowns
    {
        // Global is the global cooldown among all commands.
        public const string GLOBAL = "cooldown:global";
        // Notify is the cooldown used to determine when it can notify you that you are on cooldown again.
        public static string Notify = "cooldown:notify";
        // Vote is the cooldown used to prevent voting until it clears.
        public static string Vote = "cooldown:vote";
        // Daily is the cooldown used to prevent checking in until it clears.
        public static string Daily = "cooldown:checked_in";
    }
}
