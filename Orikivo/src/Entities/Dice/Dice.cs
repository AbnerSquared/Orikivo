using System;
using System.Collections.Generic;

namespace Orikivo
{
    // barebones class
    /// <summary>
    /// Represents an object with unique sides that can be used to randomly generate numbers.
    /// </summary>
    public class Dice : IEquatable<Dice>
    {
        private const int RAND_LENGTH = 20;

        /// <summary>
        /// Constructs a new <see cref="Dice"/> of a specified size.
        /// </summary>
        public Dice(int sides)
        {
            Size = sides;
            Length = RAND_LENGTH;
        }

        /// <summary>
        /// Constructs a new <see cref="Dice"/> with a specified size and length.
        /// </summary>
        public Dice(int size, int length)
        {
            if (length <= 0)
                throw new ArgumentException("The length specified must be greater than zero.");

            Size = size;
            Length = length;
        }

        /// <summary>
        /// Returns a six-sided <see cref="Dice"/>.
        /// </summary>
        public static Dice Default => new Dice(6);

        /// <summary>
        /// Represents the range of each side when generating new rolls.
        /// </summary>
        internal int Length { get; }
        
        /// <summary>
        /// Gets or sets a 32-bit integer that represents the number of faces that the <see cref="Dice"/> contains.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Rolls the <see cref="Dice"/>.
        /// </summary>
        public int Roll()
            => Randomizer.Roll(this);

        /// <summary>
        /// Rolls the <see cref="Dice"/> a specified number of times.
        /// </summary>
        public List<int> Roll(int times)
            => Randomizer.Roll(this, times);

        public bool Equals(Dice dice)
            => Size == dice.Size;

        public override bool Equals(object obj)
            => (obj.GetType() == typeof(Dice)) ? Equals(obj as Dice) : false;

        public override int GetHashCode()
            => Size * Length;

        public static bool operator ==(Dice d1, Dice d2)
            => d1.Size == d2.Size;

        public static bool operator !=(Dice d1, Dice d2)
            => d1.Size != d2.Size;
    }
}
