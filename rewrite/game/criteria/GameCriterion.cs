using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // base game task data that is the backbone behind every task.
    public class BaseTaskData
    {
        // the attributes to pass onto each task.
        List<GameAttribute> Attributes { get; }
        // the criteria to render a game as successful to pass onto each task.
        List<TaskCriterion> SuccessCriteria { get; }
    }

    /// <summary>
    /// The criterion that is required for a game to end.
    /// </summary>
    public class GameCriterion
    {
        public List<AttributeCriterion> Criteria { get; }

        public bool Check(Player player)
            => Check(player.Attributes);
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
