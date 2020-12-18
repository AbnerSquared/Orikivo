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
            var progress = new Dictionary<string, CriterionData>();

            foreach (Criterion criterion in quest.Criteria)
            {
                var data = new CriterionData();

                if (criterion is VarCriterion varCriterion && varCriterion.JudgeAsNew)
                    data.Value = 0;

                progress.Add(criterion.Id, data);
            }

            Id = quest.Id;
            Progress = progress;
        }

        [JsonConstructor]
        internal QuestData(string id, Dictionary<string, CriterionData> progress)
        {
            Id = id;
            Progress = progress ?? new Dictionary<string, CriterionData>();
        }

        [JsonProperty("id")]
        public string Id { get; }

        /// <summary>
        /// Keeps track of all of the required criterion for a <see cref="Quest"/>.
        /// </summary>
        [JsonProperty("progress")]
        public Dictionary<string, CriterionData> Progress { get; }
    }
}
