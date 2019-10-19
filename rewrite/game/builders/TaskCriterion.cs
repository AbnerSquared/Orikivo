using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    /// <summary>
    /// A criterion defining the success state of a GameTask.
    /// </summary>
    public class TaskCriterion
    {
        /// <summary>
        /// Creates a new TaskCriterion.
        /// </summary>
        /// <param name="attributeCriteria">A collection of attribute criteria that must be met for the criterion to return true.</param>
        /// <param name="onSuccess"></param>
        public TaskCriterion(List<AttributeCriterion> attributeCriteria, TaskQueuePacket onSuccess)
        {
            // make validity checks to make sure the objects being passed aren't empty.
            AttributeCriteria = attributeCriteria;
            OnSuccess = onSuccess;

        }

        // the list of attribute ids, with their required values
        /// <summary>
        /// A collection of attribute criterion that must be met for the criterion to return true.
        /// </summary>
        public List<AttributeCriterion> AttributeCriteria { get; } = new List<AttributeCriterion>();

        // user criteria?

        /// <summary>
        /// What the game task calls upon all of the criteria being met.
        /// </summary>
        public TaskQueuePacket OnSuccess { get; }

        public bool Check(List<GameAttribute> attributes)
        {
            foreach (AttributeCriterion criterion in AttributeCriteria)
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
