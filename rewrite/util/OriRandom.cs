using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    // a class that enhances the features of randomization
    public static class OriRandom
    {
        /// <summary>
        /// Shuffles a specified collection.
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> args)
        {
            if (args == null)
                throw new NullReferenceException("The arguments cannot be null.");

            T[] sourceArray = args.ToArray();
            int len = sourceArray.Length;
            int[] randInts = GetRandInts(len - 1, len, false);
            T[] newArray = new T[len];

            for (int i = 0; i < len; i++)
                newArray[i] = sourceArray[randInts[i]];

            return newArray.ToList();
        }

        /// <summary>
        /// Chooses an element at random from a collection.
        /// </summary>
        public static T Choose<T>(IEnumerable<T> args)
        {
            if (args == null)
                throw new Exception("The arguments specified cannot be null.");

            return args.ElementAt(RandomProvider.Instance.Next(args.Count()));
        }

        /// <summary>
        /// Takes an element at random and removes it from a specified collection.
        /// </summary>
        public static T Take<T>(List<T> args)
        {
            if (args == null)
                throw new Exception("The arguments specified cannot be null.");
            
            int j = RandomProvider.Instance.Next(args.Count());
            T obj = args.ElementAt(j);
            args.RemoveAt(j);
            return obj;
        }

        /// <summary>
        /// Takes a collection of elements at random and removes all of them from a specified collection.
        /// </summary>
        public static IEnumerable<T> TakeMany<T>(List<T> args, int times)
        {
            if (times > args.Count)
                throw new Exception("You can't take more than the collection of arguments specified.");

            List<T> elements = new List<T>();
            for (int i = 0; i < times; i++)
                elements[i] = Take(args);

            return elements;
        }

        /// <summary>
        /// Chooses a collection of elements at random from a specified collection.
        /// </summary>
        public static IEnumerable<T> ChooseMany<T> (List<T> args, int times, bool allowRepeats = false)
        {
            if (times > args.Count && !allowRepeats)
                throw new Exception("Since the chosen elements cannot be repeated, you cannot get a collection of elements larger than the one specified.");

            List<T> bag = args.ToList(); // prevents editing args.
            List<T> chosen = new List<T>();
            for (int i = 0; i < times; i++)
            {
                int j = RandomProvider.Instance.Next(bag.Count);
                chosen[i] = bag[j];
                if (!allowRepeats)
                    bag.RemoveAt(j);
            }

            return chosen;
        }

        /// <summary>
        /// Rolls a 6-sided dice and returns the result.
        /// </summary>
        /// <returns></returns>
        public static int Roll()
            => Roll(Dice.Default);

        /// <summary>
        /// Rolls a dice and returns the result.
        /// </summary>
        public static int Roll(Dice dice)
        {
            int result = (int)(Math.Truncate((double)(RandomProvider.Instance.Next(1, dice.Sides * dice.Length) / dice.Length)) % dice.Sides);
            Console.WriteLine(string.Format(OriFormat.DebugFrame, $"Rolled a {dice.Sides}-sided dice and got {result}."));
            return result;
        }

        /// <summary>
        /// Rolls a dice a specified number of times and returns all results.
        /// </summary>
        public static List<int> Roll(Dice dice, int times)
        {
            List<int> rolls = new List<int>();
            for (int i = 0; i < times; i++)
                rolls.Add(Roll(dice));
            return rolls;
        }

        /// <summary>
        /// Rolls a collection of various dice with its specified number of times and returns a dice result containing all of their rolls.
        /// </summary>
        /// <param name="rolls"></param>
        /// <returns></returns>
        public static DiceResult RollMany(params (Dice, int)[] rolls)
        {
            Dictionary<Dice, int> die = new Dictionary<Dice, int>();
            foreach ((Dice dice, int times) in rolls)
            {
                if (die.ContainsKey(dice))
                    die[dice] += times;
                else
                    die[dice] = times;
            }

            List<DiceRoll> diceRolls = new List<DiceRoll>();
            foreach ((Dice dice, int times) in die)
                diceRolls.Add(new DiceRoll(dice, times, Roll(dice, times)));
            return new DiceResult(diceRolls);
        }

        // get a list of random characters using a specified tree.
        public static string GetChars(string branch, int len)
        {
            len = len > int.MaxValue ? int.MaxValue : (len < 1 ? 1 : len);
            char[] tree = new char[len];
            for (int i = 0; i < tree.Length; i++)
                tree[i] = branch[RandomProvider.Instance.Next(branch.Length)];
            return new string(tree);
        }

        public static int[] GetRandInts(int upperBound, int len, bool allowRepeats = true)
            => GetRandInts(0, upperBound, len, allowRepeats);

        /// <summary>
        /// Gets a collection of random integers.
        /// </summary>
        public static int[] GetRandInts(int lowerBound, int upperBound, int len, bool allowRepeats = true)
        {
            len = len > int.MaxValue ? int.MaxValue : (len < 1 ? 1 : len);
            Console.WriteLine($"{lowerBound} => {upperBound}, length {len}");
            List<int> bag = Int32Utils.Increment(lowerBound, upperBound).ToList();
            Console.WriteLine(bag.WriteValues());
            int[] mix = new int[len];
            for (int i = 0; i < len; i++)
            {
                int j = RandomProvider.Instance.Next(bag.Count);
                mix[i] = bag[j];
                if (!allowRepeats)
                    bag.RemoveAt(j);
            }

            return mix;
        }

        public static OriColor GetColor()
        {
            throw new NotImplementedException();
        }

        public static int Next(Range range)
            => RandomProvider.Instance.Next(range.Min, range.Max);
    }
}
