using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Represents the result of a roll for a <see cref="Orikivo.Dice"/>.
    /// </summary>
    public class DiceRoll
    {
        /// <summary>
        /// Initializes a new <see cref="DiceRoll"/> with a specified <see cref="Orikivo.Dice"/> and result.
        /// </summary>
        public DiceRoll(Dice dice, int result) : this(dice, result.AsList()) { }

        /// <summary>
        /// Initializes a new <see cref="DiceRoll"/> with a specified <see cref="Orikivo.Dice"/> and results.
        /// </summary>
        public DiceRoll(Dice dice, IEnumerable<int> results)
        {
            Dice = dice;
            Results = results;
        }

        /// <summary>
        /// Represents the <see cref="Orikivo.Dice"/> that was used.
        /// </summary>
        public Dice Dice { get; }

        /// <summary>
        /// Represents a collection that contains the results of each roll.
        /// </summary>
        public IEnumerable<int> Results { get; }

        /// <summary>
        /// Gets a 32-bit integer that represents the total number of rolls that were executed.
        /// </summary>
        public int Count => Results.Count();

        /// <summary>
        /// Gets a 32-bit integer that represents the sum of all rolls that were executed.
        /// </summary>
        public int TotalResult => Results.Sum();
    }
}
