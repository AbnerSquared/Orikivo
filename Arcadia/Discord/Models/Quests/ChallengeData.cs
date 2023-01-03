using Newtonsoft.Json;

namespace Arcadia
{
    public class ChallengeData
    {
        public ChallengeData(Challenge challenge)
        {
            if (challenge.Criterion is VarCriterion vCriterion && vCriterion.JudgeAsNew)
            {
                Complete = false;
                Id = vCriterion.Id;
                Value = 0;
            }
        }

        [JsonConstructor]
        public ChallengeData(bool complete, string id, long value)
        {
            Complete = complete;
            Id = id;
            Value = value;
        }

        [JsonProperty("complete")]
        public bool Complete { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }
}
