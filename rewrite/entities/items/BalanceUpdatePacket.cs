namespace Orikivo
{
    public class BalanceUpdatePacket
    {
        public ulong? GuildId { get; }
        public CurrencyType Type { get; }

        // Remove is Debt
        public UpdateMethod Method { get; }
        public ulong Amount { get; }

        // Can update guild-side or globally.
    }
}
