using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    // represents an item and money cache as a gift for an objective or merit.
    public class Reward
    {
        public Dictionary<string, int> ItemIds { get; set; } = new Dictionary<string, int>();
        public ulong? Money { get; set; }
        public ExpReward Exp { get; set; }

        /// <summary>
        /// Returns all values specified within the <see cref="Reward"/> as a human-readable collection.
        /// </summary>
        public List<string> GetNames()
        {
            var values = new List<string>();

            foreach((string id, int amount) in ItemIds)
            {
                string name = Engine.GetItem(id).Name;

                if (amount > 1)
                    name += $" (x{amount:##,0})";

                values.Add(name);
            }

            values = values.OrderBy(x => x).ToList();

            if (Exp != null)
            {
                string exp = $"{Exp.Value:##,0} Exp";

                if (Exp.Type != ExpType.Global)
                    exp += $" ({Exp.Type})";

                values.Insert(0, exp);
            }

            if (Money.HasValue)
                values.Insert(0, $"{Money.Value:##,0} Orite");

            return values;
        }
    }
}
