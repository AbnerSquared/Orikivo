using System;
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

        public CasinoMode Mode => CasinoMode.Gimi;

        public bool IsSuccess => Flag.EqualsAny(GimiResultFlag.Win, GimiResultFlag.Gold);
        public GimiResultFlag Flag { get; }
        public long Reward { get; }
        public long ModifiedReward { get; private set; }

        // UNUSED
        public long Risk { get; } // the risk that was utilized

        public void Apply(ArcadeUser user)
        {
            user.AddToVar(GimiStats.TimesPlayed);

            if (Flag.EqualsAny(GimiResultFlag.Win, GimiResultFlag.Gold))
            {
                user.SetVar(GimiStats.CurrentCurseStreak, 0);
                user.SetVar(GimiStats.CurrentLossStreak, 0);
                user.SetVar(GimiStats.CurrentLossAmount, 0);

                if (Flag == GimiResultFlag.Win)
                    user.SetVar(GimiStats.CurrentGoldStreak, 0);

                user.AddToVar(GimiStats.TimesWon);
                user.AddToVar(GimiStats.TotalWon, Reward);
                user.AddToVar(GimiStats.CurrentWinStreak);
                user.AddToVar(GimiStats.CurrentWinAmount, Reward);

                if (Flag == GimiResultFlag.Gold)
                {
                    if (RandomProvider.Instance.Next(0, 1001) == 1000)
                        ItemHelper.GiveItem(user, Items.PaletteGold);

                    ItemHelper.GiveItem(user, Items.PocketLawyer);
                    user.AddToVar(GimiStats.TimesGold);
                    user.AddToVar(GimiStats.CurrentGoldStreak);

                    Var.SetIfGreater(user, GimiStats.LongestGold, GimiStats.CurrentGoldStreak);
                }

                Var.SetIfGreater(user, GimiStats.LongestWin, GimiStats.CurrentWinStreak);
                Var.SetIfGreater(user, GimiStats.LargestWin, GimiStats.CurrentWinAmount);
                user.Give(Reward, out long actual, Flag != GimiResultFlag.Gold);
                ModifiedReward = actual;
            }
            else if (Flag.EqualsAny(GimiResultFlag.Lose, GimiResultFlag.Curse))
            {
                // UPDATING STATS
                user.SetVar(GimiStats.CurrentGoldStreak, 0);
                user.SetVar(GimiStats.CurrentWinStreak, 0);
                user.SetVar(GimiStats.CurrentWinAmount, 0);

                if (Flag == GimiResultFlag.Lose)
                    user.SetVar(GimiStats.CurrentCurseStreak, 0);

                user.AddToVar(GimiStats.TimesLost);
                user.AddToVar(GimiStats.TotalLost, Reward);
                user.AddToVar(GimiStats.CurrentLossStreak);
                user.AddToVar(GimiStats.CurrentLossAmount, Reward);

                if (Flag == GimiResultFlag.Curse)
                {
                    user.AddToVar(GimiStats.TimesCursed);
                    user.AddToVar(GimiStats.CurrentCurseStreak);
                    Var.SetIfGreater(user, GimiStats.LongestCurse, GimiStats.CurrentCurseStreak);
                }

                Var.SetIfGreater(user, GimiStats.LongestLoss, GimiStats.CurrentLossStreak);
                Var.SetIfGreater(user, GimiStats.LargestLoss, GimiStats.CurrentLossAmount);
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

            user.AddToVar(GimiStats.TimesPlayed);

            if (Flag.EqualsAny(GimiResultFlag.Win, GimiResultFlag.Gold))
            {
                //if (current == 0)
                //    current = (long)GimiResultFlag.Win;

                // SETTING UP MESSAGE
                icon = "💸";
                type = "+";
                color = ImmutableColor.GammaGreen;

                // UPDATING
                Var.Clear(user,
                    GimiStats.CurrentCurseStreak,
                    GimiStats.CurrentLossStreak,
                    GimiStats.CurrentLossAmount);

                if (Flag == GimiResultFlag.Win)
                    user.SetVar(GimiStats.CurrentGoldStreak, 0);

                user.AddToVar(GimiStats.TimesWon);
                user.AddToVar(GimiStats.TotalWon, Reward);
                user.AddToVar(GimiStats.CurrentWinStreak);
                user.AddToVar(GimiStats.CurrentWinAmount, Reward);

                if (Flag == GimiResultFlag.Gold)
                {
                    icon = "💎";
                    type = "+";
                    color = GammaPalette.Glass[Gamma.Max];

                    // Try to give a gold palette
                    if (RandomProvider.Instance.Next(0, 1001) == 1000)
                        ItemHelper.GiveItem(user, Items.PaletteGold);

                    ItemHelper.GiveItem(user, Items.PocketLawyer);
                    user.AddToVar(GimiStats.TimesGold);
                    user.AddToVar(GimiStats.CurrentGoldStreak);

                    Var.SetIfGreater(user, GimiStats.LongestGold, GimiStats.CurrentGoldStreak);
                }

                Var.SetIfGreater(user, GimiStats.LongestWin, GimiStats.CurrentWinStreak);
                Var.SetIfGreater(user, GimiStats.LargestWin, GimiStats.CurrentWinAmount);

                long debt = user.Debt;
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
                    // = 9 - 2
                    value = ModifiedReward;

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
                user.SetVar(GimiStats.CurrentGoldStreak, 0);
                user.SetVar(GimiStats.CurrentWinStreak, 0);
                user.SetVar(GimiStats.CurrentWinAmount, 0);

                if (Flag == GimiResultFlag.Lose)
                {
                    user.SetVar(GimiStats.CurrentCurseStreak, 0);
                }

                user.AddToVar(GimiStats.TimesLost);
                user.AddToVar(GimiStats.TotalLost, Reward);
                user.AddToVar(GimiStats.CurrentLossStreak);
                user.AddToVar(GimiStats.CurrentLossAmount, Reward);

                if (Flag == GimiResultFlag.Curse)
                {
                    icon = "🌕";//"👁‍🗨";
                    type = "-";
                    color = GammaPalette.Alconia[Gamma.Standard];

                    user.AddToVar(GimiStats.TimesCursed);
                    user.AddToVar(GimiStats.CurrentCurseStreak);
                    Var.SetIfGreater(user, GimiStats.LongestCurse, GimiStats.CurrentCurseStreak);
                }

                Var.SetIfGreater(user, GimiStats.LongestLoss, GimiStats.CurrentLossStreak);
                Var.SetIfGreater(user, GimiStats.LargestLoss, GimiStats.CurrentLossAmount);

                long balance = user.Balance;
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

            string header = $"**{type} {icon} {value:##,0}**";
            string content = $"*\"{quote}\"*";

            embedder.Header = header;
            embedder.Color = color;
            builder.Embedder = embedder;
            builder.Content = content;

            return builder.Build();
        }
    }
}
