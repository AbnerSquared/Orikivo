using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arcadia
{
    /// <summary>
    /// Represents a seal for an <see cref="ItemData"/> stack.
    /// </summary>
    public class ItemSealData
    {
        internal ItemSealData() {}

        internal ItemSealData(string referenceId)
        {
            ReferenceId = referenceId;
        }

        [JsonConstructor]
        internal ItemSealData(string referenceId, ulong? senderId, Dictionary<string, VarProgress> toUnlock)
        {
            ReferenceId = referenceId;
            SenderId = senderId;
            ToUnlock = toUnlock;
        }

        // The ID that represents the item to display itself as, acting as a costume for the actual item
        [JsonProperty("id")]
        public string ReferenceId { get; internal set; }

        // The ID of the user that sent this item
        [JsonProperty("sender_id")]
        public ulong? SenderId { get; internal set; }

        [JsonProperty("to_unlock")]
        public Dictionary<string, VarProgress> ToUnlock { get; internal set; }
    }
}
