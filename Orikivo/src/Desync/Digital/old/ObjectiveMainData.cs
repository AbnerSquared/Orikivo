using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class ObjectiveMainData
    {
        public ObjectiveMainData() { }

        [JsonConstructor]
        internal ObjectiveMainData(int completed, DateTime lastAssigned, List<Objective> current, DateTime lastSkipped)
        {
            CompletedObjectives = completed;
            LastAssigned = lastAssigned;
            CurrentObjectives = current;
            LastSkipped = lastSkipped;
        }

        [JsonProperty("completed")]
        public int CompletedObjectives { get; private set; }

        [JsonProperty("last_assigned")]
        public DateTime LastAssigned { get; private set; }

        [JsonProperty("current")]
        public List<Objective> CurrentObjectives { get; private set; }

        [JsonProperty("last_skipped")]
        public DateTime LastSkipped { get; private set; }
    }
}
