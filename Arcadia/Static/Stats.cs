namespace Orikivo
{
    /// <summary>
    /// Contains a collection of stat identifiers.
    /// </summary>
    internal static class Stats
    {
        internal static readonly string MostMoney = "generic:most_money";
        internal static readonly string MoneyEarned = "generic:money_earned";
        internal static readonly string MoneyLost = "generic:money_lost";

        internal static readonly string MostDebt = "generic:most_debt";
        internal static readonly string DebtEarned = "generic:debt_earned";
        internal static readonly string DebtLost = "generic:debt_lost";

        internal static readonly string TimesTraded = "generic:times_traded";
        internal static readonly string ItemsSold = "generic:items_sold";
        internal static readonly string GiftsGiven = "generic:gifts_given";

        internal static readonly string DailyStreak = "generic:daily_streak";
        internal static readonly string Capacity = "generic:capacity";
    }

    /*
        
        // this applies the reward to the user.
        private static void ApplyReward(User user, Reward reward)
        {
            foreach ((string itemId, int amount) in reward.ItemIds)
                user.AddItem(itemId, amount);

            user.Balance += reward.Money.GetValueOrDefault(0);

            if (reward.Exp != null)
                user.UpdateExp(reward.Exp.Value, reward.Exp.Type);
        }

     
     
     */
}
