using Orikivo.Drawing;
using Orikivo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Represents a utility class that expands upon <see cref="Random"/>.
    /// </summary>
    public static class Randomizer
    {
        /// <summary>
        /// Shuffles a specified collection.
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> args)
        {
            if (!Check.NotNull(args))
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
        /// Selects an element at random from a collection.
        /// </summary>
        public static T Choose<T>(IEnumerable<T> args)
        {
            if (!Check.NotNull(args))
                throw new NullReferenceException("The arguments specified cannot be null.");

            return args.ElementAt(RandomProvider.Instance.Next(args.Count()));
        }

        /// <summary>
        /// Selects and removes an element at random from a collection.
        /// </summary>
        public static T Take<T>(List<T> args)
        {
            if (!Check.NotNull(args))
                throw new NullReferenceException("The arguments specified cannot be null.");
            
            int j = RandomProvider.Instance.Next(args.Count());
            T obj = args.ElementAt(j);
            args.RemoveAt(j);
            return obj;
        }

        /// <summary>
        /// Selects and removes a collection of elements at random from a collection.
        /// </summary>
        public static IEnumerable<T> TakeMany<T>(List<T> args, int times)
        {
            if (times > args.Count)
                throw new ArgumentException("You can't take more than the collection of arguments specified.");

            List<T> elements = new List<T>();
            for (int i = 0; i < times; i++)
                elements[i] = Take(args);

            return elements;
        }

        /// <summary>
        /// Selects a collection of elements at random from a collection.
        /// </summary>
        public static IEnumerable<T> ChooseMany<T> (IEnumerable<T> args, int times, bool allowRepeats = false)
        {
            if (times > args.Count() && !allowRepeats)
                throw new Exception("Since the chosen elements cannot be repeated, you cannot get a collection of elements larger than the one specified.");

            List<T> bag = args.ToList();
            List<T> chosen = new List<T>();
            for (int i = 0; i < times; i++)
            {
                int j = RandomProvider.Instance.Next(bag.Count);
                Logger.Debug($"I: {i}\nJ: {j}\nBag.Count: {bag.Count}");
                chosen.Add(bag[j]);
                if (!allowRepeats)
                    bag.RemoveAt(j);
            }

            return chosen;
        }

        /// <summary>
        /// Selects a collection of elements at random from a collection.
        /// </summary>
        public static IEnumerable<T> ChooseMany<T>(IEnumerable<T> args, int times, int maxRepeats)
        {
            if (times > (args.Count() * maxRepeats)) // TODO: Handle maxRepeatLogic
                throw new Exception("Since the chosen elements cannot be repeated, you cannot get a collection of elements larger than the one specified.");

            List<T> bag = args.ToList();
            List<T> chosen = new List<T>();
            Dictionary<T, int> repeated = new Dictionary<T, int>();
            for (int i = 0; i < times; i++)
            {
                int j = RandomProvider.Instance.Next(bag.Count);
                chosen.Add(bag[j]);

                // add a repeating listener
                if (!repeated.TryAdd(bag[j], 1))
                    repeated[bag[j]] += 1;

                // the moment it's equal to the maximum number of repeats, prevent it from being chosen again.
                if (repeated[bag[j]] >= maxRepeats)
                    bag.RemoveAt(j);
            }

            return chosen;
        }

        /// <summary>
        /// Rolls a <see cref="Dice.Default"/>.
        /// </summary>
        public static int Roll()
            => Roll(Dice.Default);

        /// <summary>
        /// Rolls a <see cref="Dice"/>.
        /// </summary>
        public static int Roll(Dice dice)
        {
            int result = (int)(Math.Truncate((double)(RandomProvider.Instance.Next(1, dice.Size * dice.Length) / dice.Length)) % dice.Size);
            Logger.Debug($"Received {result} from a {dice.Size}-sided dice.");
            return result;
        }

        /// <summary>
        /// Rolls a <see cref="Dice"/> a specified number of times.
        /// </summary>
        public static List<int> Roll(Dice dice, int times)
        {
            List<int> rolls = new List<int>();
            for (int i = 0; i < times; i++)
                rolls.Add(Roll(dice));
            return rolls;
        }

        /// <summary>
        /// Rolls a collection of various dice with its specified number of times, returning a dice result containing all of their rolls.
        /// </summary>
        public static DiceResult RollMany(params (Dice, int)[] rolls)
        {
            Dictionary<Dice, int> die = new Dictionary<Dice, int>();
            foreach ((Dice dice, int times) in rolls)
            {
                if (!die.TryAdd(dice, times))
                    die[dice] += times;
            }

            List<DiceRoll> diceRolls = new List<DiceRoll>();
            foreach ((Dice dice, int times) in die)
                diceRolls.Add(new DiceRoll(dice, Roll(dice, times)));
            return new DiceResult(diceRolls);
        }

        /// <summary>
        /// Gets a collection of random characters from a specified string.
        /// </summary>
        public static string GetChars(string branch, int len)
        {
            len = len > int.MaxValue ? int.MaxValue : (len < 1 ? 1 : len);
            char[] tree = new char[len];
            for (int i = 0; i < tree.Length; i++)
                tree[i] = branch[RandomProvider.Instance.Next(branch.Length)];
            return new string(tree);
        }

        /// <summary>
        /// Gets a collection of random full-range integers.
        /// </summary>
        public static int[] GetRandInts(int length, bool canRepeat = true)
            => GetRandInts(int.MinValue, int.MaxValue, length, canRepeat);

        /// <summary>
        /// Gets a collection of random non-negative integers within a specified upper bound.
        /// </summary>
        public static int[] GetRandInts(int upperBound, int length, bool allowRepeats = true)
            => GetRandInts(0, upperBound, length, allowRepeats);

        /// <summary>
        /// Gets a collection of random integers within a specified bound.
        /// </summary>
        public static int[] GetRandInts(int lowerBound, int upperBound, int len, bool allowRepeats = true)
        {
            len = len > int.MaxValue ? int.MaxValue : (len < 1 ? 1 : len);
            List<int> bag = Int32Utils.Increment(lowerBound, upperBound).ToList();
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

        /// <summary>
        /// Returns a random <see cref="GammaColor"/> by a raw Unicode value.
        /// </summary>
        public static GammaColor NextColor()
            => new GammaColor((uint)RandomProvider.Instance.Next(0x000000, 0xFFFFFF));

        /// <summary>
        /// Returns a random <see cref="GammaColor"/> by hue.
        /// </summary>
        public static GammaColor NextColorHue()
            => GammaColor.FromHsl(Next(RangeF.Degree), 0.0f, 1.0f);

        /// <summary>
        /// Returns a <see cref="bool"/> at random.
        /// </summary>
        /// <param name="chance">The specified chance at which the <see cref="bool"/> will be true.</param>
        public static bool NextBool(float chance = 0.5f)
            => RandomProvider.Instance.NextDouble() > chance;

        /// <summary>
        /// Returns a random 32-bit integer that is within the specified range.
        /// </summary>
        public static int Next(RangeF range)
        {
            range = RangeF.Truncate(range);
            return RandomProvider.Instance.Next((int)range.Min, (int)range.Max);
        }
    }
}
