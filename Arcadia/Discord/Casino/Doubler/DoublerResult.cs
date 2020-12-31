using System;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia.Casino
{
    public class DoublerResult : ICasinoResult
    {
        public DoublerResult(long wager, long reward, DoublerResultFlag flag, int expectedTick, int actualTick, float multiplier)
        {
            Wager = wager;
            Reward = reward;
            Flag = flag;
            ExpectedTick = expectedTick;
            ActualTick = actualTick;
            Multiplier = multiplier;
        }

        public CasinoMode Mode => CasinoMode.Tick;

        public int ExpectedTick { get; }

        public int ActualTick { get; }

        public long Wager { get; }

        public float Multiplier { get; }

        public long Reward { get; }

        public bool IsSuccess => Flag.EqualsAny(DoublerResultFlag.Win, DoublerResultFlag.Exact);

        public DoublerResultFlag Flag { get; }

        public Message ApplyAndDisplay(ArcadeUser user)
        {
            ImmutableColor color = ImmutableColor.GammaGreen;
            string type = IsSuccess ? "+" : "-";
            long value = IsSuccess ? Reward : Wager;

            user.AddToVar(Stats.Doubler.TotalBet, Wager);
            user.AddToVar(Stats.Doubler.TimesPlayed);

            user.ChipBalance -= Wager;

            switch (Flag)
            {
                case DoublerResultFlag.Exact:
                case DoublerResultFlag.Win:
                    user.SetVar(Stats.Doubler.CurrentLossStreak, 0);

                    user.AddToVar(Stats.Doubler.TimesWon);
                    user.AddToVar(Stats.Doubler.TotalWon, Reward);

                    if (ExpectedTick == ActualTick)
                        user.AddToVar(Stats.Doubler.TimesWonExact);

                    user.AddToVar(Stats.Doubler.CurrentWinStreak);
                    user.AddToVar(Stats.Doubler.CurrentWinAmount, Reward);

                    Var.SetIfGreater(user, Stats.Doubler.LongestWin, Stats.Doubler.CurrentWinStreak);
                    Var.SetIfGreater(user, Stats.Doubler.LargestWin, Stats.Doubler.CurrentWinAmount);
                    Var.SetIfGreater(user, Stats.Doubler.LargestWinSingle, Reward);
                    break;

                case DoublerResultFlag.Lose:
                    Var.Clear(user, Stats.Doubler.CurrentWinStreak, Stats.Doubler.CurrentWinAmount);
                    user.AddToVar(Stats.Doubler.TimesLost);
                    user.AddToVar(Stats.Doubler.CurrentLossStreak);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Flag));
            }

            if (IsSuccess)
                user.ChipBalance += CurrencyHelper.BoostValue(user, Reward, BoostTarget.Chips);

            string header = $"**{type} 🧩 {value:##,0}**";

            string content = Replies.GetReply(Flag, user, this);
                //GetQuote(ExpectedTick, ActualTick, Multiplier, Reward, IsSuccess);

            var embedder = new Embedder
            {
                Header = header,
                Color = color
            };

            var builder = new MessageBuilder();

            builder.WithEmbedder(embedder)
                .WithContent(content);

            return builder.Build();
        }
    }
}
