using System;
using System.Collections.Generic;

namespace Orikivo
{
    // where extra balance is stored.
    public class OffshoreBalance
    {
        public ulong Savings { get; set; }
        public ulong Checkings { get; set; }
        public DateTime LastSavingsUsage { get; set; }

        public int WithdrawLimit { get; set; }
        public int Withdraws { get; set; }
        public List<BankStatement> History { get; set; }

        public void WithdrawCheckings(OldAccount a, ulong v)
        {

        }

        public void WithdrawSavings(OldAccount a, ulong v)
        {

        }

        public void InsertBalance(OldAccount a, BankStorage b)
            => Insert(a.Balance, b);

        public void Insert(ulong v, BankStorage b)
        {
            switch (b)
            {
                case BankStorage.Checkings:
                    InsertCheckings(v);
                    return;
                default:
                    InsertSavings(v);
                    return;
            }
        }

        private void InsertCheckings(ulong v)
            => Checkings += v;

        private void InsertSavings(ulong v)
            => Savings += v;
    }
}
