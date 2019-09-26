using System;
using System.Collections.Generic;

namespace Orikivo.Helpers
{
    /// <summary>
    /// Represents a collection of methods dedicated to the Random class.
    /// </summary>
    public static class RandomHelper
    {
        // This is how many different numbers can equal to that side of the dice.
        private const int DiceRange = 20;

        private const int CoinRange = 1000;

        private const int MIN_COIN_RANGE = 1;
        private const int MAX_COIN_RANGE = int.MaxValue;

        // 2,147,483,647 - Max int.
        private static int EnsureCoin(this int i)
            => i.InRange(MIN_COIN_RANGE, MAX_COIN_RANGE);



        private static int GetSide(Random rng, Dice d)
        {
            int limit = GetRange(d);
            int pure = PureRoll(rng, d);

            if (pure > (limit / 2))
                return FromUpper(pure, d);
            else
                return FromLower(pure, d);
        }

        private static int PureRoll(Random rng, Dice d)
        {
            return rng.Next(1, GetRange(d));
        }

        private static int FromLower(int pure, Dice d)
        {
            int side = 0;

            for (int i = 0; i < (d.Sides - 1); i++)
            {
                int lower = GetRange(i) + 1;
                int upper = GetRange(i + 1);

                if (pure.IsInRange(lower, upper))
                {
                    side = i + 1;
                    break;
                }
            }

            return side;
        }

        private static int FromUpper(int pure, Dice d)
        {
            int side = 0;
            for (int i = d.Sides; i > 0; i--)
            {
                int upper = GetRange(i);
                int lower = GetRange(i - 1) + 1;

                if (pure.IsInRange(lower, upper))
                {
                    side = i;
                    break;
                }
            }
            return side;
        }

        public static int Roll(Random rng)
        {
            Dice d = Dice.Default;
            return Roll(rng, d);
        }

        public static int Roll(Random rng, Dice d)
        {
            int raw = rng.Next(1, GetRange(d));
            int side = GetSide(rng, d);

            return side;
        }

        public static List<int> RollMany(Random rng, Dice d, int amount)
        {
            List<int> rolls = new List<int>();
            for (int i = 0; i < amount; i++)
            {
                rolls.Add(Roll(rng, d));
            }

            return rolls;
        }

        private static int GetRange(int sides)
        {
            return DiceRange * sides;
        }

        private static int GetRange(Dice dice)
        {
            return DiceRange * dice.Sides;
        }

        // true.... heads
        // false.... tails
        public static bool FlipCoin(Random rng)
        {
            int flip = rng.Next(0, CoinRange);
            return flip > (CoinRange / 2);
        }

        public static bool FlipCoin(Random rng, int range)
        {
            range = range.EnsureCoin();
            int flip = rng.Next(0, range);
            return flip > (int)Math.Round((double)range / 2);
        }
    }
}