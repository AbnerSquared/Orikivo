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

        private static string GetQuote(int expectedTick, int actualTick, float multiplier, long reward, bool isSuccess)
        {
            int diff = Math.Abs(actualTick - expectedTick);

            if (!isSuccess)
            {
                return Randomizer.ChooseAny($"The dying tick was **{actualTick}**.",
                    "Ouch. Sorry about that.",
                    $"You missed a chance at **{Icons.Chips} {reward:##,0}**.",
                    $"Don't forget that your **Chips** ({Icons.Chips}) are always taken at the start.",
                    diff == expectedTick
                        ? "There was never a successful tick in this round."
                        : $"You were **{diff}** {Format.TryPluralize("tick", diff)} away from **{expectedTick}**.");
            }

            return Randomizer.ChooseAny("Nicely done!",
                multiplier > 5
                    ? $"You beat the odds, landing you a **x{multiplier:##,0.#}** return!"
                    : $"You have received a **x{multiplier:##,0.#}** return.",
                diff == 0
                    ? $"The dying tick was exactly **{expectedTick}**. Good guessing!"
                    : $"In this round, the dying tick was **{actualTick}**.");
        }

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

                    StatHelper.SetIfGreater(user, TickStats.LongestWin, TickStats.CurrentWinStreak);
                    StatHelper.SetIfGreater(user, TickStats.LargestWin, TickStats.CurrentWinAmount);
                    StatHelper.SetIfGreater(user, TickStats.LargestWinSingle, Reward);
                    break;

                case TickResultFlag.Lose:
                    StatHelper.Clear(user, TickStats.CurrentWinStreak, TickStats.CurrentWinAmount);
                    user.AddToVar(TickStats.TimesLost);
                    user.AddToVar(TickStats.CurrentLossStreak);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Flag));
            }

            if (IsSuccess)
                user.ChipBalance += ItemHelper.BoostValue(user, Reward, BoosterType.Chips);

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
