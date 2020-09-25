using System.Collections.Generic;
using System.Text;
using Arcadia.Services;
using Orikivo;

namespace Arcadia
{
    public static class ProfileViewer
    {
        public static string View(ArcadeUser user, ArcadeContext ctx)
        {
            var details = new StringBuilder();

            details.AppendLine($"> **{user.Username}**");
            details.AppendLine($"> Joined: **{Format.Date(user.CreatedAt, '.')}**");

            if (user.Balance > 0 || user.Debt > 0 || user.ChipBalance > 0)
            {
                details.AppendLine("\n> **Wallet**");

                if (user.Balance > 0 || user.Debt > 0)
                {
                    string icon = user.Balance > 0 ? Icons.Balance : Icons.Debt;
                    long value = user.Balance > 0 ? user.Balance : user.Debt;
                    string id = user.Balance > 0 ? Vars.Balance : Vars.Debt;
                    int position = Leaderboard.GetPosition(ctx.Data.Users.Values.Values, user, id);
                    string pos = "";
                    if (position < 4)
                        pos = $" (#**{position:##,0}** global)";

                    details.AppendLine($"> {icon} **{value:##,0}**{pos}");
                }

                if (user.ChipBalance > 0)
                {
                    int position = Leaderboard.GetPosition(ctx.Data.Users.Values.Values, user, Vars.Chips);
                    string pos = "";
                    if (position < 4)
                        pos = $" (#**{position:##,0}** global)";
                    details.AppendLine($"> {Icons.Chips} **{user.ChipBalance:##,0}**{pos}");
                }
            }

            if (user.Items.Count > 0)
            {
                details.AppendLine($"\n> **Inventory**");
                details.AppendLine($"> **{user.Items.Count}** used {Format.TryPluralize("slot", user.Items.Count)}");
            }

            List<string> randomStats = GetRandomStats(user, 3);


            if (Check.NotNullOrEmpty(randomStats))
                return details.ToString();

            details.AppendLine($"\n> **Random Statistics**");

            foreach (string stat in randomStats)
                details.AppendLine($"> **{Var.WriteName(stat)}**: {Var.WriteValue(user, stat)}");

            return details.ToString();
        }

        private static List<string> GetRandomStats(ArcadeUser user, int count)
        {
            var chosen = new List<string>();
            string stat = StatHelper.GetRandomStat(user, chosen);

            while (!string.IsNullOrWhiteSpace(stat))
            {
                if (chosen.Count >= count)
                    break;

                chosen.Add(stat);
                stat = StatHelper.GetRandomStat(user, chosen);
            }

            return chosen;
        }
    }
}
