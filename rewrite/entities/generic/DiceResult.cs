using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
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

        public List<DiceRoll> Rolls { get; } = new List<DiceRoll>();
        public List<Dice> GetDie() 
            => Rolls.Select(x => x.Dice).ToList();

        public int GetDiceUsed()
            => OriMath.Add(Rolls.Select(x => x.Times));
        public int GetTotalResult()
            => OriMath.Add(Rolls.Select(x => x.TotalResult));

    }
}
