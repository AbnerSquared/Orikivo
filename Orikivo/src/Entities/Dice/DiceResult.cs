using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // barebones class
    // dice result holds the dice used, how many times it was used, and the resulting rolls for all rolls.
    public class DiceResult
    {
        public DiceResult(params DiceRoll[] rolls)
        {
            Rolls = rolls.ToList();
        }

        public DiceResult(List<DiceRoll> rolls)
        {
            Rolls = rolls ?? new List<DiceRoll>();
        }

        public List<DiceRoll> Rolls { get; }

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
