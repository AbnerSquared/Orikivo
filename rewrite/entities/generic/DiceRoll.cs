using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// Defines the result of a roll for a specified dice.
    /// </summary>
    public class DiceRoll
    {
        public DiceRoll(Dice dice, int result) : this(dice, 1, result.AsList()) { }
        public DiceRoll(Dice dice, int times, List<int> results)
        {
            Dice = dice;
            Times = times;
            Results = results;
        }
        public Dice Dice { get; }
        public int Times { get; }
        public List<int> Results { get; }
        public int TotalResult => Results.Sum();
    }
}
