using System;
using System.Text;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia.Casino
{
    public enum RouletteResultFlag
    {
        Win = 1,
        Lose = 2
    }

    public class Roulette
    {
        public static readonly long MaxWager = 1000;
        public static readonly int[] Pockets =
        {
            0,
            32, 15, 19,  4, 21,  2, 25, 17, 34,  6, 27, 13,
            36, 11, 30,  8, 23, 10,  5, 24, 16, 33,  1, 20,
            14, 31,  9, 22, 18, 29,  7, 28, 12, 35,  3, 26
        };

        public static bool IsGreen(int index)
            => index == 0;

        public static bool IsRed(int index)
            => index != 0 && (index % 2 == 1);

        public static bool IsBlack(int index)
            => index != 0 && (index % 2 == 0);

        public static RoulettePocketColor GetColor(int index)
        {
            if (IsBlack(index))
                return RoulettePocketColor.Black;

            if (IsRed(index))
                return RoulettePocketColor.Red;

            if (IsGreen(index))
                return RoulettePocketColor.Green;

            Logger.Debug($"{index}");
            throw new Exception("Unknown slot color");
        }

        public static int GetSlotIndex()
            => RandomProvider.Instance.Next(0, 37);

            // GREEN
            // 0

            // REDS
            // 32, 19, 21, 25, 34, 27, 36, 30, 23,  5, 16,  1, 14,  9, 18,  7, 12,  3

            // BLACKS
            // 15,  4,  2, 17,  6, 13, 11,  8, 10, 24, 33, 20, 31, 22, 29, 28, 35, 26

        public static RouletteResult Next(ArcadeUser user, RouletteBetMode mode, long wager)
        {
            int index = RandomProvider.Instance.Next(0, 37);
            int pocket = Pockets[index];
            RoulettePocketColor color = GetColor(index);
            bool isSuccess = JudgeBet(mode, index);
            RouletteResultFlag flag = isSuccess ? RouletteResultFlag.Win : RouletteResultFlag.Lose;
            float multiplier = GetPayout(mode) + 1;
            long reward = GetReward(wager, mode);
            return new RouletteResult
            {
                Color = color,
                Index = index,
                Pocket = pocket,
                Mode = mode,
                Wager = wager,
                IsSuccess = isSuccess,
                Flag = flag,
                Multiplier = multiplier,
                Reward = reward
            };
        }

        private static long GetReward(long bet, RouletteBetMode mode)
        {
            return bet + (long) Math.Floor(bet * (double)GetPayout(mode));
        }

        private static int GetPayout(RouletteBetMode mode)
        {
            switch (mode)
            {
                case RouletteBetMode.Red:
                case RouletteBetMode.Black:
                case RouletteBetMode.Low:
                case RouletteBetMode.High:
                case RouletteBetMode.Odd:
                case RouletteBetMode.Even:
                    return 1;

                case RouletteBetMode.DozenA:
                case RouletteBetMode.DozenB:
                case RouletteBetMode.DozenC:
                    return 2;

                case RouletteBetMode.Basket:
                    return 6;

                case RouletteBetMode.Green:
                    return 35;

                default:
                    throw new Exception("Unknown bet mode");
            }
        }

        private static bool JudgeBet(RouletteBetMode mode, int index)
        {
            return mode switch
            {
                RouletteBetMode.Green => IsGreen(index),
                RouletteBetMode.Red => IsRed(index),
                RouletteBetMode.Black => IsBlack(index),
                RouletteBetMode.High => !IsGreen(index) && Pockets[index] >= 19,
                RouletteBetMode.Low => !IsGreen(index) && Pockets[index] < 19,
                RouletteBetMode.Odd => !IsGreen(index) && Pockets[index] % 2 == 1,
                RouletteBetMode.Even => !IsGreen(index) && Pockets[index] % 2 == 0,
                RouletteBetMode.DozenA => Pockets[index] >= 1 && Pockets[index] <= 12,
                RouletteBetMode.DozenB => Pockets[index] >= 13 && Pockets[index] <= 24,
                RouletteBetMode.DozenC => Pockets[index] >= 25 && Pockets[index] <= 36,
                RouletteBetMode.Basket => Pockets[index].EqualsAny(0, 1, 2, 3),
                _ => throw new Exception("Unknown bet mode")
            };
        }
    }

    public enum RouletteBetMode
    {
        Green = 1,
        Red = 2,
        Black = 3,
        High = 4,
        Low = 5, // 
        Odd = 6, // 1 to 1
        Even = 7, // 1 to 1
        DozenA = 8,
        DozenB = 9,
        DozenC = 10,
        Basket = 11
    }

    public enum RoulettePocketColor
    {
        Red = 1,
        Black = 2,
        Green = 4
    }

    public static class RouletteStats
    {
        public static readonly string TimesPlayed = "roulette:times_played";
        public static readonly string TimesWon = "roulette:times_won";
        public static readonly string TimesWonGreen = "roulette:times_won_green";
        public static readonly string TimesLost = "roulette:times_lost";
        public static readonly string TotalWon = "roulette:total_won";
        public static readonly string TotalLost = "roulette:total_lost";
        public static readonly string MostWon = "roulette:most_won";
    }

    public class RouletteResult
    {
        public long Wager;
        public RouletteBetMode Mode;
        public long Reward;
        public float Multiplier;
        public RouletteResultFlag Flag;
        public bool IsSuccess;
        public int Index;
        public int Pocket;
        public RoulettePocketColor Color;

        /*
        public void Apply(ArcadeUser user)
        {

        }*/

        // TODO: Make a mathematical operator
        public static int Mod(int x, int m)
        {
            if (m < 0)
                m = -m;

            int r = x % m;
            return r < 0 ? r + m : r;
        }

        public Message ApplyAndDisplay(ArcadeUser user)
        {
            var result = new StringBuilder();

            user.ChipBalance -= Wager;

            user.AddToVar(RouletteStats.TimesPlayed);

            if (IsSuccess)
            {
                user.AddToVar(RouletteStats.TimesWon);
                user.AddToVar(RouletteStats.TotalWon, Reward - Wager);
                Var.SetIfGreater(user, RouletteStats.MostWon, Reward - Wager);

                if (Pocket == 0)
                    user.AddToVar(RouletteStats.TimesWonGreen);
            }
            else
            {
                user.AddToVar(RouletteStats.TimesLost);
                user.AddToVar(RouletteStats.TotalLost, Wager);
            }

            if (IsSuccess)
                user.ChipBalance += Reward;

            result.AppendLine($"> **Roulette**");
            result.AppendLine($"> Betting on **{HumanizeBet(Mode)}**");

            int pIndex = Mod((Index - 1), 37);
            string previous = $"{GetPocketIcon(Roulette.GetColor(pIndex))} {Roulette.Pockets[pIndex]}";
            string pocket = $"{GetPocketIcon(Color)} **{Pocket}**";
            int nIndex = Mod((Index + 1), 37);
            string next = $"{GetPocketIcon(Roulette.GetColor(nIndex))} {Roulette.Pockets[nIndex]}";

            string outcome = IsSuccess
                ? $"> + {Icons.Chips} **{Reward:##,0}** (x**{Multiplier:##,0.##}**)"
                : $"> - {Icons.Chips} **{Wager:##,0}**";

            // TODO: Add more replies
            string reply = IsSuccess ? "You have won." : "You have lost.";

            result.AppendLine($"\n> {previous}\n> {pocket}\n> {next}\n");
            result.AppendLine($"{outcome}");
            result.AppendLine($"> {reply}");

            return new Message(result.ToString());
        }

        private static string GetPocketIcon(RoulettePocketColor color)
        {
            return color switch
            {
                RoulettePocketColor.Green => "🟩",
                RoulettePocketColor.Red => "🟥",
                RoulettePocketColor.Black => "⬛", //"🔳",
                _ => throw new Exception("Unknown color")
            };
        }

        private static string HumanizeBet(RouletteBetMode mode)
        {
            return mode switch
            {
                RouletteBetMode.DozenA => "1st Dozen",
                RouletteBetMode.DozenB => "2nd Dozen",
                RouletteBetMode.DozenC => "3rd Dozen",
                _ => mode.ToString()
            };
        }
    }
}
