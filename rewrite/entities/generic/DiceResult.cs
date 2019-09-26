using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // dice result holds the dice used, how many times it was used, and the resulting rolls for all rolls.
    public class DiceResult
    {
        public DiceResult(int result, params (Dice, int)[] die)
        {
            Die = new List<(Dice, int)>();
            die.ToList().ForEach(x => Die.Add(x));
        }

        public List<(Dice, int)> Die { get; }
    }
}
