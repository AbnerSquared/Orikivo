using System;
using System.Text;
using Orikivo;

namespace Arcadia.Casino
{
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

            user.AddToVar(Stats.Roulette.TimesPlayed);

            if (IsSuccess)
            {
                user.AddToVar(Stats.Roulette.TimesWon);
                user.AddToVar(Stats.Roulette.TotalWon, Reward - Wager);
                Var.SetIfGreater(user, Stats.Roulette.MostWon, Reward - Wager);

                if (Pocket == 0)
                    user.AddToVar(Stats.Roulette.TimesWonGreen);
            }
            else
            {
                user.AddToVar(Stats.Roulette.TimesLost);
                user.AddToVar(Stats.Roulette.TotalLost, Wager);
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