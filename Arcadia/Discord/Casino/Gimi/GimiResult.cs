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

        public long Reward { get; private set; }

        public long Risk { get; }

        public void Apply(ArcadeUser user)
        {
            Var.Add(user, 1, GimiStats.TimesPlayed);

            switch (Flag)
            {
                case GimiResultFlag.Win:
                case GimiResultFlag.Gold:
                    Var.Clear(user, GimiStats.CurrentCurseStreak, GimiStats.CurrentLossStreak, GimiStats.CurrentLossAmount);
                    Var.Add(user, 1, GimiStats.TimesWon, GimiStats.CurrentWinStreak);
                    Var.Add(user, Reward, GimiStats.TotalWon, GimiStats.CurrentWinAmount);
                    Var.SetIfGreater(user, GimiStats.LongestWin, GimiStats.CurrentWinStreak);
                    Var.SetIfGreater(user, GimiStats.LargestWin, GimiStats.CurrentWinAmount);

                    if (Flag == GimiResultFlag.Gold)
                    {
                        ItemHelper.GiveItem(user, Items.PocketLawyer);

                        if (RandomProvider.Instance.Next(0, 1001) == 1000)
                            ItemHelper.GiveItem(user, Items.PaletteGold);

                        Var.Add(user, 1, GimiStats.TimesGold, GimiStats.CurrentGoldStreak);
                        Var.SetIfGreater(user, GimiStats.LongestGold, GimiStats.CurrentGoldStreak);
                    }
                    else
                    {
                        Var.Clear(user, GimiStats.CurrentGoldStreak);
                        Reward = CurrencyHelper.BoostValue(user, Reward, BoostType.Money);
                    }

                    user.Give(Reward);
                    break;

                case GimiResultFlag.Lose:
                case GimiResultFlag.Curse:
                    Var.Clear(user, GimiStats.CurrentGoldStreak, GimiStats.CurrentWinStreak, GimiStats.CurrentWinAmount);
                    Var.Add(user, 1, GimiStats.TimesLost, GimiStats.CurrentLossStreak);
                    Var.Add(user, Reward, GimiStats.TotalLost, GimiStats.CurrentLossAmount);
                    Var.SetIfGreater(user, GimiStats.LongestLoss, GimiStats.CurrentLossStreak);
                    Var.SetIfGreater(user, GimiStats.LargestLoss, GimiStats.CurrentLossAmount);

                    if (Flag == GimiResultFlag.Curse)
                    {
                        Var.Add(user, 1, GimiStats.TimesCursed, GimiStats.CurrentCurseStreak);
                        Var.SetIfGreater(user, GimiStats.LongestCurse, GimiStats.CurrentCurseStreak);
                    }
                    else
                    {
                        Var.Clear(user, GimiStats.CurrentCurseStreak);
                        Reward = CurrencyHelper.BoostValue(user, Reward, BoostType.Money);
                    }

                    user.Take(Reward);
                    break;
            }
        }

        public Message ApplyAndDisplay(ArcadeUser user)
        {
            var builder = new MessageBuilder();
            var embedder = new Embedder();

            string icon = "💸";
            string type = "+";
            string quote = Replies.GetReply(Flag, user, this);
            long value = Reward;
            ImmutableColor color = ImmutableColor.GammaGreen;

            Var.Add(user, 1, GimiStats.TimesPlayed);

            switch (Flag)
            {
                case GimiResultFlag.Win:
                case GimiResultFlag.Gold:
                    Var.Clear(user, GimiStats.CurrentCurseStreak, GimiStats.CurrentLossStreak, GimiStats.CurrentLossAmount);
                    Var.Add(user, 1, GimiStats.TimesWon, GimiStats.CurrentWinStreak);
                    Var.Add(user, Reward, GimiStats.TotalWon, GimiStats.CurrentWinAmount);
                    Var.SetIfGreater(user, GimiStats.LongestWin, GimiStats.CurrentWinStreak);
                    Var.SetIfGreater(user, GimiStats.LargestWin, GimiStats.CurrentWinAmount);

                    if (Flag == GimiResultFlag.Gold)
                    {
                        icon = "💎";
                        type = "+";
                        color = GammaPalette.Glass[Gamma.Max];

                        ItemHelper.GiveItem(user, Items.PocketLawyer);

                        if (RandomProvider.Instance.Next(0, 1001) == 1000)
                            ItemHelper.GiveItem(user, Items.PaletteGold);

                        Var.Add(user, 1, GimiStats.TimesGold, GimiStats.CurrentGoldStreak);
                        Var.SetIfGreater(user, GimiStats.LongestGold, GimiStats.CurrentGoldStreak);
                    }
                    else
                    {
                        Var.Clear(user, GimiStats.CurrentGoldStreak);
                        Reward = CurrencyHelper.BoostValue(user, Reward, BoostType.Money);
                    }
                    long debt = user.Debt;
                    user.Give(Reward);

                    if (debt > Reward)
                    {
                        icon = "📃";
                        type = "-";
                        quote = Replies.Recover.Length > 0 ? (string)Randomizer.Choose(Replies.Recover) : Replies.RecoverGeneric;
                    }
                    else if (debt > 0 && Reward - debt == 0)
                    {
                        icon = "📧";
                        type = "";
                        quote = Replies.EvenGeneric;
                    }

                    break;

                case GimiResultFlag.Lose:
                case GimiResultFlag.Curse:
                    type = "-";
                    color = ImmutableColor.NeonRed;

                    Var.Clear(user, GimiStats.CurrentGoldStreak, GimiStats.CurrentWinStreak, GimiStats.CurrentWinAmount);
                    Var.Add(user, 1, GimiStats.TimesLost, GimiStats.CurrentLossStreak);
                    Var.Add(user, Reward, GimiStats.TotalLost, GimiStats.CurrentLossAmount);
                    Var.SetIfGreater(user, GimiStats.LongestLoss, GimiStats.CurrentLossStreak);
                    Var.SetIfGreater(user, GimiStats.LargestLoss, GimiStats.CurrentLossAmount);

                    if (Flag == GimiResultFlag.Curse)
                    {
                        icon = "🌕";
                        type = "-";
                        color = GammaPalette.Alconia[Gamma.Standard];

                        Var.Add(user, 1, GimiStats.TimesCursed, GimiStats.CurrentCurseStreak);
                        Var.SetIfGreater(user, GimiStats.LongestCurse, GimiStats.CurrentCurseStreak);
                    }
                    else
                    {
                        Var.Clear(user, GimiStats.CurrentCurseStreak);
                        Reward = CurrencyHelper.BoostValue(user, Reward, BoostType.Money);
                    }

                    long balance = user.Balance;
                    user.Take(Reward);

                    if (balance < Reward)
                    {
                        icon = "📃";
                        type = "+";
                        value = Reward - balance;
                        quote = Replies.Debt.Length > 0 ? (string)Randomizer.Choose(Replies.Debt) : Replies.DebtGeneric;
                    }
                    else if (balance > 0 && Reward - balance == 0)
                    {
                        icon = "📧";
                        value = Reward - balance;
                        type = "";
                        quote = Replies.EvenGeneric;
                    }
                    break;
            }

            if (!string.IsNullOrWhiteSpace(type))
                type += ' ';

            string header = $"**{type}{icon} {value:##,0}**";
            string content = $"*\"{quote}\"*";

            embedder.Header = header;
            embedder.Color = color;
            builder.Embedder = embedder;
            builder.Content = content;

            return builder.Build();
        }
    }
}
