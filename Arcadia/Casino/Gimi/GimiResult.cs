using Orikivo.Drawing;
using Orikivo;

namespace Arcadia.Casino
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
        public long ModifiedReward { get; private set; }

        // UNUSED
        public long Risk { get; } // the risk that was utilized

        public void Apply(ArcadeUser user)
        {
            user.UpdateStat(GimiStats.TimesPlayed);

            if (Flag.EqualsAny(GimiResultFlag.Win, GimiResultFlag.Gold))
            {
                user.SetStat(GimiStats.CurrentCurseStreak, 0);
                user.SetStat(GimiStats.CurrentLossStreak, 0);
                user.SetStat(GimiStats.CurrentLossAmount, 0);

                if (Flag == GimiResultFlag.Win)
                    user.SetStat(GimiStats.CurrentGoldStreak, 0);

                user.UpdateStat(GimiStats.TimesWon);
                user.UpdateStat(GimiStats.TotalWon, Reward);
                user.UpdateStat(GimiStats.CurrentWinStreak);
                user.UpdateStat(GimiStats.CurrentWinAmount, Reward);

                if (Flag == GimiResultFlag.Gold)
                {
                    ItemHelper.GiveItem(user, ItemHelper.GetItem(Items.PocketLawyer));
                    user.UpdateStat(GimiStats.TimesGold);
                    user.UpdateStat(GimiStats.CurrentGoldStreak);

                    StatHelper.SetIfGreater(user, GimiStats.LongestGold, GimiStats.CurrentGoldStreak);
                }

                StatHelper.SetIfGreater(user, GimiStats.LongestWin, GimiStats.CurrentWinStreak);
                StatHelper.SetIfGreater(user, GimiStats.LargestWin, GimiStats.CurrentWinAmount);
                user.Give(Reward, out long actual, Flag != GimiResultFlag.Gold);
                ModifiedReward = actual;
            }
            else if (Flag.EqualsAny(GimiResultFlag.Lose, GimiResultFlag.Curse))
            {
                // UPDATING STATS
                user.SetStat(GimiStats.CurrentGoldStreak, 0);
                user.SetStat(GimiStats.CurrentWinStreak, 0);
                user.SetStat(GimiStats.CurrentWinAmount, 0);

                if (Flag == GimiResultFlag.Lose)
                    user.SetStat(GimiStats.CurrentCurseStreak, 0);

                user.UpdateStat(GimiStats.TimesLost);
                user.UpdateStat(GimiStats.TotalLost, Reward);
                user.UpdateStat(GimiStats.CurrentLossStreak);
                user.UpdateStat(GimiStats.CurrentLossAmount, Reward);

                if (Flag == GimiResultFlag.Curse)
                {
                    user.UpdateStat(GimiStats.TimesCursed);
                    user.UpdateStat(GimiStats.CurrentCurseStreak);
                    StatHelper.SetIfGreater(user, GimiStats.LongestCurse, GimiStats.CurrentCurseStreak);
                }

                StatHelper.SetIfGreater(user, GimiStats.LongestLoss, GimiStats.CurrentLossStreak);
                StatHelper.SetIfGreater(user, GimiStats.LargestLoss, GimiStats.CurrentLossAmount);
                user.Take(Reward, out long actual, Flag != GimiResultFlag.Curse);
                ModifiedReward = actual;
            }
        }

        // Apply the GimiResult to the user that executed it.
        public Message ApplyAndDisplay(ArcadeUser user)
        {
            // MESSAGE FRAMEWORK
            var builder = new MessageBuilder();
            var embedder = new Embedder();

            string icon = "💸";
            string type = "+";
            string quote = Replies.GetReply(Flag, user, this);
            long value = Reward;
            ImmutableColor color = ImmutableColor.GammaGreen;

            //long current = user.GetStat(GimiStats.CurrentType);

            user.UpdateStat(GimiStats.TimesPlayed);

            if (Flag.EqualsAny(GimiResultFlag.Win, GimiResultFlag.Gold))
            {
                //if (current == 0)
                //    current = (long)GimiResultFlag.Win;

                // SETTING UP MESSAGE
                icon = "💸";
                type = "+";
                color = ImmutableColor.GammaGreen;

                // UPDATING
                StatHelper.Clear(user,
                    GimiStats.CurrentCurseStreak,
                    GimiStats.CurrentLossStreak,
                    GimiStats.CurrentLossAmount);

                if (Flag == GimiResultFlag.Win)
                    user.SetStat(GimiStats.CurrentGoldStreak, 0);

                user.UpdateStat(GimiStats.TimesWon);
                user.UpdateStat(GimiStats.TotalWon, Reward);
                user.UpdateStat(GimiStats.CurrentWinStreak);
                user.UpdateStat(GimiStats.CurrentWinAmount, Reward);

                if (Flag == GimiResultFlag.Gold)
                {
                    icon = "💎";
                    type = "+";
                    color = GammaPalette.Glass[Gamma.Max];

                    ItemHelper.GiveItem(user, ItemHelper.GetItem(Items.PocketLawyer));
                    user.UpdateStat(GimiStats.TimesGold);
                    user.UpdateStat(GimiStats.CurrentGoldStreak);

                    StatHelper.SetIfGreater(user, GimiStats.LongestGold, GimiStats.CurrentGoldStreak);
                }

                StatHelper.SetIfGreater(user, GimiStats.LongestWin, GimiStats.CurrentWinStreak);
                StatHelper.SetIfGreater(user, GimiStats.LargestWin, GimiStats.CurrentWinAmount);

                long debt = (long)user.Debt;
                // UPDATING BALANCE
                user.Give(Reward, out value, Flag != GimiResultFlag.Gold);
                ModifiedReward = value;

                // 3 > 9
                if (debt > ModifiedReward) // if there's a reward, but the person is still in debt
                {
                    icon = "📃";
                    type = "-";
                    quote = Replies.Recover.Length > 0 ? (string)Randomizer.Choose(Replies.Recover) : Replies.RecoverGeneric;
                }

                // 3 > 0
                // if there's still debt, but it was paid off, show the remainder as what they gained.
                else if (debt > 0)
                {
                    // 9 - 3
                    value = ModifiedReward - debt;

                    if (value == 0)
                    {
                        icon = "📧";
                        type = "";
                        quote = Replies.EvenGeneric;
                    }
                }
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
                }

                user.UpdateStat(GimiStats.TimesLost);
                user.UpdateStat(GimiStats.TotalLost, Reward);
                user.UpdateStat(GimiStats.CurrentLossStreak);
                user.UpdateStat(GimiStats.CurrentLossAmount, Reward);

                if (Flag == GimiResultFlag.Curse)
                {
                    icon = "🌕";//"👁‍🗨";
                    type = "-";
                    color = GammaPalette.Alconia[Gamma.Standard];

                    user.UpdateStat(GimiStats.TimesCursed);
                    user.UpdateStat(GimiStats.CurrentCurseStreak);
                    StatHelper.SetIfGreater(user, GimiStats.LongestCurse, GimiStats.CurrentCurseStreak);
                }

                StatHelper.SetIfGreater(user, GimiStats.LongestLoss, GimiStats.CurrentLossStreak);
                StatHelper.SetIfGreater(user, GimiStats.LargestLoss, GimiStats.CurrentLossAmount);

                long balance = (long) user.Balance;
                // UPDATING BALANCE
                user.Take(Reward, out value, Flag != GimiResultFlag.Curse);
                ModifiedReward = value;

                if (balance < ModifiedReward) // if there's a loss larger than the person's balance, show the remainder as debt (ONLY IF > THAN 0)
                {
                    icon = "📃";
                    type = "+";
                    value = ModifiedReward - balance;
                    quote = Replies.Debt.Length > 0 ? (string)Randomizer.Choose(Replies.Debt) : Replies.DebtGeneric;
                }
                else if (balance > 0)
                {
                    // 9 - 3

                    if (ModifiedReward - balance == 0)
                    {
                        icon = "📧";
                        value = ModifiedReward - balance;
                        type = "";
                        quote = Replies.EvenGeneric;
                    }
                }
            }

            string header = $"**{type} {icon} {value:##,0.###}**";
            string content = $"*\"{quote}\"*";

            embedder.Header = header;
            embedder.Color = color;
            builder.Embedder = embedder;
            builder.Content = content;

            return builder.Build();
        }
    }
}
