using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class ObjectiveData
    {
        public ObjectiveData(Objective objective)
        {
            var trackers = new Dictionary<string, int>();

            foreach (ObjectiveCriterion criterion in objective.Criteria)
            {
                trackers.Add(criterion.Id, 0);
            }

            Trackers = trackers;
        }

        [JsonConstructor]
        public ObjectiveData(Dictionary<string, int> trackers)
        {
            Trackers = trackers ?? new Dictionary<string, int>();
        }

        /// <summary>
        /// Stores a list of all trackers an objective uses.
        /// </summary>
        [JsonProperty("trackers")]
        public Dictionary<string, int> Trackers { get; set; }
    }
}
