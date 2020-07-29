using Newtonsoft.Json;
using System.Collections.Generic;

namespace Arcadia
{
    // listen to stat IDs, and every time those are updated, you change the ones on the quest
    /// <summary>
    /// Represents a the data of a <see cref="Quest"/>.
    /// </summary>
    public class QuestData
    {
        public QuestData(Quest quest)
        {
            var progress = new Dictionary<string, long>();

            foreach (StatCriterion criterion in quest.Criteria)
                progress.Add(criterion.Id, 0);

            Id = quest.Id;
            Progress = progress;
        }

        [JsonConstructor]
        internal QuestData(string id, Dictionary<string, long> progress)
        {
            Id = id;
            Progress = progress ?? new Dictionary<string, long>();
        }

        public string Id { get; }

        /// <summary>
        /// Keeps track of all of the required criterion for a <see cref="Quest"/>.
        /// </summary>
        [JsonProperty("progress")]
        public Dictionary<string, long> Progress { get; }
    }
}
