using Newtonsoft.Json;

namespace Orikivo
{
    // TODO: Calculate % of all users that have unlocked this merit.
    public class MeritInfo
    {
        [JsonConstructor]
        internal MeritInfo(string id, string name, MeritGroup group, VarCriterion[] criteria, MeritRewardInfo reward)
        {
            Id = id;
            Name = name;
            Group = group;
            Criteria = criteria;
            OnSuccess = reward;
        }

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("group")]
        public MeritGroup Group { get; } = MeritGroup.Misc;

        [JsonProperty("criteria")]
        public VarCriterion[] Criteria { get; }

        [JsonProperty("reward")]
        public MeritRewardInfo OnSuccess { get; }
    }
}
