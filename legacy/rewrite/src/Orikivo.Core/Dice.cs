using Newtonsoft.Json;
using Orikivo.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orikivo
{
    /// <summary>
    /// Represents a collection of Dice.
    /// </summary>
    public class DiceBatch
    {

    }

    /// <summary>
    /// Represents an object used to randomly select numbers.
    /// </summary>
    public class Dice
    {
        [JsonIgnore]
        private const int DefaultSides = 6;

        [JsonIgnore]
        public const int MaximumSides = 100;

        [JsonIgnore]
        public const int MinimumSides = 1;

        [JsonIgnore]
        public static Dice Default = new Dice();

        public Dice (int sides)
        {
            sides = Ensure(sides);
            Sides = sides;
        }

        public Dice()
        {
            Sides = DefaultSides;
        }

        public int Sides { get; set; }
        //public int Amount { get; set; }

        public static bool TryParse(string s, out List<Dice> d)
        {
            d = new List<Dice>();
            Regex pattern = new Regex(@"\d*d\d{1,3}");
            List<Match> matches = pattern.Matches(s).ToList();
            if (matches.Count == 0)
                return false;

            foreach (Match m in matches)
            {
                m.Value.Debug();
                string[] info = m.Value.Split('d');
                string.Join('\n', info).Debug();
                if (info.Length == 2)
                {
                    int amount = int.Parse(info[0]);
                    int sides = int.Parse(info[1]);
                    d.Add(new Dice(sides));
                }
                if (info.Length == 1)
                {
                    int sides = int.Parse(info[0]);
                    d.Add(new Dice(sides));
                }
            }

            return true;
        }

        private int Ensure(int sides)
        {
            return sides.InRange(MinimumSides, MaximumSides);
        }

        public DiceRoll Roll()
        {
            return new DiceRoll(this);
        }

        public DiceRollBatch RollMany(int amount)
        {
            return new DiceRollBatch(this, amount);
        }
        public override string ToString()
        {
            return $"D{Sides}";
        }
    }

    public class DiceRoll
    {
        public DiceRoll(Dice dice)
        {
            Dice = dice;
            Result = RandomProvider.Instance.Roll(Dice);
            Max = (ulong)Dice.Sides;
        }

        public Dice Dice { get; } // dice used to roll
        public int Result { get; } // total roll score
        public ulong Max { get; } // max roll possible

        public override string ToString()
        {
            return $"{Dice.ToString()} | {Result}/{Max}";
        }

    }

    public class DiceRollBatch
    {
        [JsonIgnore]
        private const int DefaultAmount = 1;

        [JsonIgnore]
        public const int MaximumAmount = 100;

        [JsonIgnore]
        public const int MinimumAmount = 1;

        public DiceRollBatch(Dice dice, int amount)
        {
            Dice = dice;
            amount = Ensure(amount);
            Amount = amount;
            Rolls = RandomProvider.Instance.RollMany(Dice, amount);
            Result = Rolls.Tally();
            Max = (ulong)(Amount * Dice.Sides);
        }

        /// <summary>
        /// The dice that used in the batch roll.
        /// </summary>
        public Dice Dice { get; }

        /// <summary>
        /// The amount of dice that was rolled.
        /// </summary>
        public int Amount { get; } // amount of dice rolled.

        /// <summary>
        /// A collection of all rolls.
        /// </summary>
        public List<int> Rolls { get; } // a list of all rolls

        /// <summary>
        /// The total count of all dice rolls.
        /// </summary>
        public ulong Result { get; } // total roll score

        /// <summary>
        /// The total count of all dice sides.
        /// </summary>
        public ulong Max { get; } // max roll possible

        
        /// <summary>
        /// Makes sure the dice roll limit is in range.
        /// </summary>
        private int Ensure(int amount)
        {
            return amount.InRange(MinimumAmount, MaximumAmount);
        }

        public override string ToString()
        {
            return $"{(Amount > 1 ? $"{Amount}" : "")}{Dice.ToString()} | {Result}/{Max}";
        }

    }
}