namespace Orikivo
{
    public class BalanceUpdatePacket
    {
        public ulong? GuildId { get; }
        public CurrencyType Type { get; }
        public ulong Amount { get; }

        // Can update guild-side or globally.
    }
}
