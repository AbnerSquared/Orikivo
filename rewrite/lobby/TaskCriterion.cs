using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // a task criteria used to check if attributes are valid
    public class TaskCriterion
    {
        public TaskCriterion(List<AttributeCriterion> criteria, GameRoute onSuccess)
        {
            // make validity checks to make sure the objects being passed aren't empty.
            Requirements = criteria;
            OnSuccess = onSuccess;

        }

        // the list of attribute ids, with their required values
        public List<AttributeCriterion> Requirements { get; }
        public GameRoute OnSuccess { get; }

        public bool Check(List<GameAttribute> attributes)
        {
            foreach (AttributeCriterion criterion in Requirements)
            {
                if (!attributes.Any(x => x.Id == criterion.RequiredId))
                    throw new Exception("The criteria is checking for an attribute that doesn't exist.");
                if (!criterion.Check(attributes.First(x => x.Id == criterion.RequiredId)))
                    return false;
            }
            return true;
        }
    }
}
