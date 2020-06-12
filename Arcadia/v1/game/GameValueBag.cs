using Orikivo;
using System;

namespace Arcadia.Old
{
    // a collection of random integers
    /// <summary>
    /// A collection of random values that can be used to set random values for attributes.
    /// </summary>
    public class GameValueBag
    {
        public GameValueBag(string name, params int[] values)
        {
            Name = name;
            if (values.Length < 2)
                throw new ArgumentException("At least two values must be specified in order to create a value bag.");

            Values = DefaultValues = values;
        }

        public string Name { get; }
        public string Id => $"bag.{Name}";

        public int[] Values { get; internal set; }
        public int[] DefaultValues { get; }

        public int Size => DefaultValues.Length;
        public int CurrentCount => Values.Length;

        /// <summary>
        /// Takes a random value from the collection of values. If the values are empty, it is reset.
        /// </summary>
        public int Take()
        {
            int pos = RandomProvider.Instance.Next(Values.Length);
            int value = Values[pos];
            if (Values.Length - 1 > 0)
            {
                int[] newValues = new int[Values.Length - 1];

                bool skip = false;
                for (int i = 0; i < Values.Length; i++)
                {
                    if (i == pos)
                    {
                        skip = true;
                        continue;
                    }

                    newValues[i - (skip ? 1 : 0)] = Values[i];
                }

                Values = newValues;
            }
            else
                Values = DefaultValues;

            return value;
        }

        /// <summary>
        /// Randomly selects a value from the collection of values.
        /// </summary>
        public int Select()
        {
            return Values[RandomProvider.Instance.Next()];
        }
    }

    // create a function determining what to do upon starting from a specific task.
}
