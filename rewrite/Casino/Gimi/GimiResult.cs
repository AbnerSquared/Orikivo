using Orikivo.Casino;
using Orikivo.Drawing;
using Orikivo.Unstable;
using System.Collections.Generic;

namespace Orikivo
{
    public class GimiResult : ICasinoResult
    {
        public GimiResult(long reward, GimiResultFlag flag)
        {
            Reward = reward;
            Flag = flag;
        }

        public bool IsSuccess => Flag.EqualsAny(GimiResultFlag.Win, GimiResultFlag.Gold);
        public GimiResultFlag Flag { get; }
        public long Reward { get; }

        // UNUSED
        public long Risk { get; } // the risk that was utilized
        public long EarnExpander { get; } // the earn expander level

        // Apply the GimiResult to the user that executed it.
        public Message ApplyAndDisplay(User user)
        {
            // MESSAGE FRAMEWORK
            MessageBuilder builder = new MessageBuilder();
            Embedder embedder = new Embedder();

            string icon = "💸";
            string type = "+";
            string quote = CasinoReplies.NextReply(Flag);
            long value = Reward;
            GammaColor color = GammaColor.GammaGreen;

            user.UpdateStat(GimiStats.TimesPlayed, 1);

            if (Flag.EqualsAny(GimiResultFlag.Win, GimiResultFlag.Gold))
            {
                // SETTING UP MESSAGE
                icon = "💸";
                type = "+";
                color = GammaColor.GammaGreen;

                // UPDATING STATS
                user.SetStat(GimiStats.CurrentCurseStreak, 0);
                user.SetStat(GimiStats.CurrentLossStreak, 0);
                user.SetStat(GimiStats.CurrentLossAmount, 0);

                if (Flag == GimiResultFlag.Win)
                    user.SetStat(GimiStats.CurrentGoldStreak, 0);

                user.UpdateStat(GimiStats.TimesWon, 1);
                user.UpdateStat(GimiStats.TotalWon, Reward);
                user.UpdateStat(GimiStats.CurrentWinStreak, 1);
                user.UpdateStat(GimiStats.CurrentWinAmount, Reward);

                if (Flag == GimiResultFlag.Gold)
                {
                    icon = "💎";
                    type = "+";
                    color = GammaPalette.Glass[Gamma.Max];

                    user.UpdateStat(GimiStats.TimesGold, 1);
                    user.UpdateStat(GimiStats.CurrentGoldStreak, 1);

                    if (user.GetStat(GimiStats.CurrentGoldStreak) > user.GetStat(GimiStats.LongestGoldStreak))
                        user.SetStat(GimiStats.LongestGoldStreak, user.GetStat(GimiStats.CurrentGoldStreak));
                }

                if (user.GetStat(GimiStats.CurrentWinStreak) > user.GetStat(GimiStats.LongestWinStreak))
                {
                    user.SetStat(GimiStats.LongestWinStreak, user.GetStat(GimiStats.CurrentWinStreak));
                    user.SetStat(GimiStats.LongestWinAmount, user.GetStat(GimiStats.CurrentWinAmount));
                }

                if (user.GetStat(GimiStats.CurrentWinAmount) > user.GetStat(GimiStats.LargestWinAmount))
                {
                    user.SetStat(GimiStats.LargestWinStreak, user.GetStat(GimiStats.CurrentWinStreak));
                    user.SetStat(GimiStats.LargestWinAmount, user.GetStat(GimiStats.CurrentWinAmount));
                }

                if ((long)user.Debt >= Reward) // if there's a reward, but the person is still in debt
                {
                    icon = "📃";
                    type = "-";
                }

                // UPDATING BALANCE
                user.Give(Reward);
            }
            else if (Flag.EqualsAny(GimiResultFlag.Lose, GimiResultFlag.Curse))
            {
                icon = "💸";
                type = "-";
                color = GammaColor.NeonRed;

                // UPDATING STATS
                user.SetStat(GimiStats.CurrentGoldStreak, 0);
                user.SetStat(GimiStats.CurrentWinStreak, 0);
                user.SetStat(GimiStats.CurrentWinAmount, 0);

                if (Flag == GimiResultFlag.Lose)
                {
                    user.SetStat(GimiStats.CurrentCurseStreak, 0);

                    if ((long)user.Balance < Reward) // if there's a loss larger than the person's balance, show the remainder as debt (ONLY IF > THAN 0)
                    {
                        icon = "📃";
                        type = "+";
                        value = Reward - (long)user.Balance;
                        quote = CasinoReplies.Debt.Length > 0 ?
                            Randomizer.Choose(CasinoReplies.Debt)
                            : CasinoReplies.DebtGeneric;
                    }
                }

                user.UpdateStat(GimiStats.TimesLost, 1);
                user.UpdateStat(GimiStats.TotalLost, Reward);
                user.UpdateStat(GimiStats.CurrentLossStreak, 1);
                user.UpdateStat(GimiStats.CurrentLossAmount, Reward);

                if (Flag == GimiResultFlag.Curse)
                {
                    icon = "🌑";//"👁‍🗨";
                    type = "-";
                    color = GammaPalette.Alconia[Gamma.StandardDim];

                    user.UpdateStat(GimiStats.TimesCursed, 1);
                    user.UpdateStat(GimiStats.CurrentCurseStreak, 1);

                    if (user.GetStat(GimiStats.CurrentCurseStreak) > user.GetStat(GimiStats.LongestCurseStreak))
                        user.SetStat(GimiStats.LongestCurseStreak, user.GetStat(GimiStats.CurrentCurseStreak));
                }

                if (user.GetStat(GimiStats.CurrentLossStreak) > user.GetStat(GimiStats.LongestLossStreak))
                {
                    user.SetStat(GimiStats.LongestLossStreak, user.GetStat(GimiStats.CurrentLossStreak));
                    user.SetStat(GimiStats.LongestLossAmount, user.GetStat(GimiStats.CurrentLossAmount));
                }

                if (user.GetStat(GimiStats.CurrentLossAmount) > user.GetStat(GimiStats.LargestLossAmount))
                {
                    user.SetStat(GimiStats.LargestLossStreak, user.GetStat(GimiStats.CurrentLossStreak));
                    user.SetStat(GimiStats.LargestLossAmount, user.GetStat(GimiStats.CurrentLossAmount));
                }

                

                // UPDATING BALANCE
                user.Take(Reward);


            }

            string header = $"**{type} {icon} {value.ToString("##,0.###")}**";
            string content = $"*\"{quote}\"*";

            embedder.Header = header;
            embedder.Color = color;
            builder.Embedder = embedder;
            builder.Content = content;

            return builder.Build();
        }
    }
}
