using System;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia
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

        public int ExpectedTick { get; }
        
        public int ActualTick { get; }
        
        public long Wager { get; }
        
        public float Multiplier { get; }
        
        public long Reward { get; }

        public bool IsSuccess => Flag == TickResultFlag.Win;

        public TickResultFlag Flag { get; }

        private static string GetQuote(int expectedTick, int actualTick, float multiplier, long reward, bool isSuccess)
        {
            int diff = Math.Abs(actualTick - expectedTick);

            if (!isSuccess)
            {
                // loss
                return Randomizer.ChooseAny($"The dying tick was **{actualTick}**.",
                    "Ouch. Sorry about that.",
                    $"You missed a chance at **🧩 {reward.ToString("##,0")}**.",
                    "Don't forget that your **Chips** (🧩) are always taken at the start.",
                    diff == expectedTick ? "There was never a successful tick in this round." : $"You were **{diff}** {OriFormat.TryPluralize("tick", diff)} away from **{expectedTick}**.");
            }

            // win
            return Randomizer.ChooseAny(
                "Nicely done!",
                (multiplier > 5 ? $"You beat the odds, landing you a **x{multiplier.ToString("##,0.##")}** return!"
                    : $"You have received a **x{multiplier.ToString("##,0.##")}** return."),
                (diff == 0 ? $"The dying tick was exactly **{expectedTick}**. Good guessing!"
                    : $"In this round, the dying tick was **14**."));
        }

        public Message ApplyAndDisplay(ArcadeUser user)
        {
            var builder = new MessageBuilder();
            var embedder = new Embedder();

            ImmutableColor color = ImmutableColor.GammaGreen;
            string icon = "🧩";
            string type = IsSuccess ? "+" : "-";
            string quote = GetQuote(ExpectedTick, ActualTick, Multiplier, Reward, IsSuccess);
            long value = IsSuccess ? Reward : Wager;

            user.UpdateStat(TickStats.TotalBet, Wager);
            user.UpdateStat(TickStats.TimesPlayed);

            user.ChipBalance -= (ulong)Wager;

            if (Flag == TickResultFlag.Win)
            {
                user.SetStat(TickStats.CurrentLossStreak, 0);

                user.UpdateStat(TickStats.TimesWon);
                user.UpdateStat(TickStats.TotalWon, Reward);

                if (ExpectedTick == ActualTick)
                    user.UpdateStat(TickStats.TimesWonExact);

                user.UpdateStat(TickStats.CurrentWinStreak);
                user.UpdateStat(TickStats.CurrentWinAmount, Reward);

                StatHelper.SetIfGreater(user, TickStats.LongestWin, TickStats.CurrentWinStreak);
                StatHelper.SetIfGreater(user, TickStats.LargestWin, TickStats.CurrentWinAmount);
                StatHelper.SetIfGreater(user, TickStats.LargestWinSingle, Reward);
            }
            else if (Flag == TickResultFlag.Lose)
            {
                user.SetStat(TickStats.CurrentWinStreak, 0);
                user.SetStat(TickStats.CurrentWinAmount, 0);

                user.UpdateStat(TickStats.TimesLost);
                user.UpdateStat(TickStats.CurrentLossStreak);
            }

            if (IsSuccess)
            {
                user.ChipBalance += (ulong)Reward;
            }

            string header = $"**{type} {icon} {value.ToString("##,0")}**";
            string content = quote;

            embedder.Header = header;
            embedder.Color = color;
            builder.Embedder = embedder;
            builder.Content = content;
            return builder.Build();
        }
    }
}
