using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class DiceResult
    {
        public DiceResult(List<DiceRoll> rolls)
        {
            if (Check.NotNullOrEmpty(rolls))
                throw new ArgumentException("The specified collection of dice rolls is null or empty");

            Rolls = rolls;
        }

        public IReadOnlyList<DiceRoll> Rolls { get; }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of all of the dice that were used on this result.
        /// </summary>
        public IEnumerable<Dice> GetDice() 
            => Rolls.Select(x => x.Dice);

        public int GetRollCount()
            => Rolls.Select(x => x.Count).Sum();

        public int GetTotalResult()
            => Rolls.Select(x => x.TotalResult).Sum();

    }
}
