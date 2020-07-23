using Arcadia.Casino;
using Discord;
using Orikivo.Drawing;
using Orikivo.Desync;
using Orikivo;

namespace Arcadia
{
    public class GimiResult : ICasinoResult
    {
        public GimiResult(long reward, GimiResultFlag flag, long risk)
        {
            Reward = reward;
            Flag = flag;
            Risk = risk;
        }

        public bool IsSuccess => Flag.EqualsAny(GimiResultFlag.Win, GimiResultFlag.Gold);
        public GimiResultFlag Flag { get; }
        public long Reward { get; }

        // UNUSED
        public long Risk { get; } // the risk that was utilized

        // Apply the GimiResult to the user that executed it.
        public Message ApplyAndDisplay(ArcadeUser user)
        {
            // MESSAGE FRAMEWORK
            var builder = new MessageBuilder();
            var embedder = new Embedder();

            string icon = "💸";
            string type = "+";
            string quote = CasinoReplies.GetReply(Flag, user);
            long value = Reward;
            ImmutableColor color = ImmutableColor.GammaGreen;

            //long current = user.GetStat(GimiStats.CurrentType);

            user.UpdateStat(GimiStats.TimesPlayed, 1);

            if (Flag.EqualsAny(GimiResultFlag.Win, GimiResultFlag.Gold))
            {
                //if (current == 0)
                //    current = (long)GimiResultFlag.Win;

                // SETTING UP MESSAGE
                icon = "💸";
                type = "+";
                color = ImmutableColor.GammaGreen;

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

                    ItemHelper.GiveItem(user, ItemHelper.GetItem(Items.PocketLawyer));
                    user.UpdateStat(GimiStats.TimesGold, 1);
                    user.UpdateStat(GimiStats.CurrentGoldStreak, 1);

                    StatHelper.SetIfGreater(user, GimiStats.LongestGold, GimiStats.CurrentGoldStreak);
                }

                StatHelper.SetIfGreater(user, GimiStats.LongestWin, GimiStats.CurrentWinStreak);
                StatHelper.SetIfGreater(user, GimiStats.LargestWin, GimiStats.CurrentWinAmount);

                // 3 > 9
                if ((long)user.Debt > Reward) // if there's a reward, but the person is still in debt
                {
                    icon = "📃";
                    type = "-";
                    quote = CasinoReplies.Recover.Length > 0 ? (string)Randomizer.Choose(CasinoReplies.Recover) : CasinoReplies.RecoverGeneric;
                }

                // 3 > 0
                // if there's still debt, but it was paid off, show the remainder as what they gained.
                else if ((long) user.Debt > 0)
                {
                    // 9 - 3
                    value = Reward - (long)user.Debt;

                    if (value == 0)
                    {
                        icon = "📧";
                        type = "";
                        quote = CasinoReplies.EvenGeneric;
                    }
                }

                // UPDATING BALANCE
                user.Give(Reward);
            }
            else if (Flag.EqualsAny(GimiResultFlag.Lose, GimiResultFlag.Curse))
            {
                icon = "💸";
                type = "-";
                color = ImmutableColor.NeonRed;

                //if (current == 0)
                //    current = (long)GimiResultFlag.Lose;

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
                        quote = CasinoReplies.Debt.Length > 0 ? (string)Randomizer.Choose(CasinoReplies.Debt) : CasinoReplies.DebtGeneric;
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
                    color = GammaPalette.Alconia[Gamma.Standard];

                    user.UpdateStat(GimiStats.TimesCursed, 1);
                    user.UpdateStat(GimiStats.CurrentCurseStreak, 1);
                    StatHelper.SetIfGreater(user, GimiStats.LongestCurse, GimiStats.CurrentCurseStreak);
                }

                StatHelper.SetIfGreater(user, GimiStats.LongestLoss, GimiStats.CurrentLossStreak);
                StatHelper.SetIfGreater(user, GimiStats.LargestLoss, GimiStats.CurrentLossAmount);

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
