namespace Orikivo
{
    // TODO: Modify its purpose.
    public class GuildBalanceInfo
    {
        public GuildBalanceInfo()
        {
            Balance = 0;
            Debt = 0;
        }

        public GuildBalanceInfo(ulong balance, ulong debt)
        {
            Balance = balance;
            Debt = debt;
        }

        public ulong Balance { get; set; }
        public ulong Debt { get; set; }

        public void Update(ulong bal = 0, ulong debt = 0)
        {
            Balance += bal;
            Debt += debt;
        }
    }
}
