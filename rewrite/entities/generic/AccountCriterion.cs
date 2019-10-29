using Newtonsoft.Json;

namespace Orikivo
{
    public class AccountCriterion
    {
        [JsonConstructor]
        internal AccountCriterion(string varId, int value)
        {
            VarId = varId;
            Value = value;
        }

        /// <summary>
        /// The unique identifier of any user variable.
        /// </summary>
        [JsonProperty("var")]
        public string VarId { get; }

        [JsonProperty("value")]
        public int Value { get; }
    }
}
