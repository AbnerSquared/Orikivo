using Orikivo.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Represents a utility class that expands upon <see cref="Random"/>.
    /// </summary>
    internal static class Randomizer
    {
        public static T ChooseOrDefault<T>(in IEnumerable<T> args)
        {
            if (!(args?.Any() ?? false))
                return default;

            // If there's only one element, just return that
            if (args.Count() == 1)
                return args.First();

            return args.ElementAt(RandomProvider.Instance.Next(args.Count()));
        }

        /// <summary>
        /// Selects an element at random from a collection.
        /// </summary>
        public static T Choose<T>(in IEnumerable<T> args)
        {
            if (!(args?.Any() ?? false))
                throw new NullReferenceException("The arguments specified cannot be null or empty.");

            // If there's only one element, just return that
            if (args.Count() == 1)
                return args.First();

            return args.ElementAt(RandomProvider.Instance.Next(args.Count()));
        }

        public static T Choose<T>(in IEnumerable<T> args, Func<T, bool> predicate)
        {
            return Choose(args.Where(predicate));
        }

        public static T ChooseOrDefault<T>(in IEnumerable<T> args, Func<T, bool> predicate)
        {
            return ChooseOrDefault(args.Where(predicate));
        }

        public static T ChooseAny<T>(params T[] args)
            => Choose(args);

        /// <summary>
        /// Selects and removes an element at random from a collection.
        /// </summary>
        public static T Take<T>(List<T> args)
        {
            if (!(args?.Any() ?? false))
                throw new NullReferenceException("The arguments specified cannot be null.");
            
            int j = RandomProvider.Instance.Next(args.Count);
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

            var elements = new List<T>();

            for (int i = 0; i < times; i++)
                elements[i] = Take(args);

            return elements;
        }

        /// <summary>
        /// Selects a collection of elements at random from a collection.
        /// </summary>
        public static IEnumerable<T> ChooseMany<T> (in IEnumerable<T> args, int times, bool allowRepeats = false)
        {
            if (times > args.Count() && !allowRepeats)
                throw new Exception("Since the chosen elements cannot be repeated, you cannot get a collection of elements larger than the one specified.");

            var chosen = new List<T>();
            var bag = args.ToList();

            for (int i = 0; i < times; i++)
            {
                int j = RandomProvider.Instance.Next(bag.Count);
                chosen.Add(bag.ElementAt(j));

                if (!allowRepeats)
                    bag.RemoveAt(j);
            }

            return chosen;
        }

        /// <summary>
        /// Selects a collection of elements at random from a collection.
        /// </summary>
        public static IEnumerable<T> ChooseMany<T>(in IEnumerable<T> args, int times, int maxRepeats)
        {
            if (times > (args.Count() * maxRepeats)) // TODO: Handle maxRepeatLogic
                throw new Exception("Since the chosen elements cannot be repeated, you cannot get a collection of elements larger than the one specified.");

            var chosen = new List<T>();
            var bag = args.ToList();
            var repeated = new Dictionary<int, int>();

            for (int i = 0; i < times; i++)
            {
                int j = RandomProvider.Instance.Next(bag.Count);
                chosen.Add(bag[j]);

                // add a repeating listener
                if (!repeated.TryAdd(j, 1))
                    repeated[j] += 1;

                // the moment it's equal to the maximum number of repeats, prevent it from being chosen again.
                if (repeated[j] >= maxRepeats)
                    bag.RemoveAt(j);
            }

            return chosen;
        }

        /// <summary>
        /// Gets a collection of random characters from a specified string.
        /// </summary>
        public static string GetChars(string branch, int len)
        {
            len = len < 1 ? 1 : len;
            char[] tree = new char[len];

            for (int i = 0; i < tree.Length; i++)
                tree[i] = branch[RandomProvider.Instance.Next(branch.Length)];

            return new string(tree);
        }

        /// <summary>
        /// Returns a random <see cref="ImmutableColor"/> by a raw Unicode value.
        /// </summary>
        public static ImmutableColor NextColor()
            => new ImmutableColor((uint) RandomProvider.Instance.Next(0x000000, 0xFFFFFF));

        /// <summary>
        /// Returns a random <see cref="ImmutableColor"/> by hue.
        /// </summary>
        public static ImmutableColor NextColorHue(float saturation = 0, float lightness = 1)
            => ImmutableColor.FromHsl(Next(RangeF.Degree), saturation, lightness);

        /// <summary>
        /// Returns a <see cref="bool"/> at random.
        /// </summary>
        /// <param name="chance">The specified chance at which the <see cref="bool"/> will be true.</param>
        public static bool NextBool(float chance = 0.5f)
            => RandomProvider.Instance.NextDouble() <= chance;

        /// <summary>
        /// Returns a random 32-bit integer that is within the specified range.
        /// </summary>
        public static int Next(RangeF range)
        {
            range = RangeF.Truncate(range);
            return RandomProvider.Instance.Next((int)range.Min, (int)range.Max);
        }

        public static AngleF NextAngle()
        {
            return RangeF.Convert(0, 1, 0, 360, (float) RandomProvider.Instance.NextDouble());
        }
    }
}
