using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Old
{
    /// <summary>
    /// The criterion that is required for a game to end.
    /// </summary>
    public class GameCriterion
    {
        public List<AttributeCriterion> Criteria { get; }

        public bool Check(Player player)
            => Check(player.Attributes);
        // TODO: This can be split into a separate static utility service.
        public bool Check(List<GameAttribute> attributes)
        {
            foreach (AttributeCriterion criterion in Criteria)
            {
                if (!attributes.Any(x => x.Id == criterion.RequiredId))
                    throw new Exception($"The GameCriterion is requesting an attribute ({criterion.RequiredId}) that doesn't exist.");
                if (!criterion.Check(attributes.First(x => x.Id == criterion.RequiredId)))
                {
                    Console.WriteLine($"An attribute failed to meet the criterion's required value. {criterion.RequiredId}: {criterion.RequiredValue}");
                    return false;
                }
                Console.WriteLine($"Criterion has been met. ({criterion.RequiredId})");
            }
            Console.WriteLine("All criteria have been met.");
            return true;
        }
    }
}
