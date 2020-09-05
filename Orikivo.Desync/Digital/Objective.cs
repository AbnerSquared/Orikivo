using System.Collections.Generic;

namespace Orikivo.Desync
{

    public class Objective
    {
        public string Id { get; set; }

        /// <summary>
        /// Represents the ID of the flag to be added when this objective is completed.
        /// </summary>
        public string SuccessId { get; set; }
        
        public ObjectiveRank Rank { get; set; }

        public List<ObjectiveCriterion> Criteria { get; }
    }
}
