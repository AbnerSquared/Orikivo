using System;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia.Casino
{
    public class TickResult : ICasinoResult
    {
        public TickResult(long wager, long reward, TickResultFlag flag, int expectedTick, int actualTick, float multiplier)
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

        public bool IsSuccess => Flag.EqualsAny(TickResultFlag.Win, TickResultFlag.Exact);

        public TickResultFlag Flag { get; }

        public Message ApplyAndDisplay(ArcadeUser user)
        {
            ImmutableColor color = ImmutableColor.GammaGreen;
            string type = IsSuccess ? "+" : "-";
            long value = IsSuccess ? Reward : Wager;

            user.AddToVar(TickStats.TotalBet, Wager);
            user.AddToVar(TickStats.TimesPlayed);

            user.ChipBalance -= Wager;

            switch (Flag)
            {
                case TickResultFlag.Exact:
                case TickResultFlag.Win:
                    user.SetVar(TickStats.CurrentLossStreak, 0);

                    user.AddToVar(TickStats.TimesWon);
                    user.AddToVar(TickStats.TotalWon, Reward);

                    if (ExpectedTick == ActualTick)
                        user.AddToVar(TickStats.TimesWonExact);

                    user.AddToVar(TickStats.CurrentWinStreak);
                    user.AddToVar(TickStats.CurrentWinAmount, Reward);

                    Var.SetIfGreater(user, TickStats.LongestWin, TickStats.CurrentWinStreak);
                    Var.SetIfGreater(user, TickStats.LargestWin, TickStats.CurrentWinAmount);
                    Var.SetIfGreater(user, TickStats.LargestWinSingle, Reward);
                    break;

                case TickResultFlag.Lose:
                    Var.Clear(user, TickStats.CurrentWinStreak, TickStats.CurrentWinAmount);
                    user.AddToVar(TickStats.TimesLost);
                    user.AddToVar(TickStats.CurrentLossStreak);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Flag));
            }

            if (IsSuccess)
                user.ChipBalance += ItemHelper.BoostValue(user, Reward, BoostType.Chips);

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
