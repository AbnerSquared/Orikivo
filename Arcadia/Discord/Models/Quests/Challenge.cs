using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arcadia
{
    /// <summary>
    /// Represents a basic objective.
    /// </summary>
    public class Challenge
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int Difficulty { get; set; } // The higher the number, the harder it is
        // You want the difficulty to increase as the user completes more challenge sets
        // For each set completed, increase the cap of where the difficulty cuts off.

        public CriteriaTriggers Triggers { get; set; }

        public Criterion Criterion { get; internal set; }
        public IEnumerator<Challenge> GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

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
