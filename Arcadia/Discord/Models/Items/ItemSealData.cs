using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arcadia
{
    /// <summary>
    /// Represents a seal for an <see cref="ItemData"/> stack.
    /// </summary>
    public class ItemSealData
    {
        internal ItemSealData()
        {
            ReferenceId = "$_seal";
        }

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

        // NOTE: Think of it as a costume for the current item stack
        /// <summary>
        /// Represents the ID of the <see cref="Item"/> to showcase this <see cref="ItemData"/> as.
        /// </summary>
        [JsonProperty("id")]
        public string ReferenceId { get; internal set; }

        // The ID of the user that sent this item
        /// <summary>
        /// Represents the ID of the user that sent this <see cref="ItemData"/>.
        /// </summary>
        [JsonProperty("sender_id")]
        public ulong? SenderId { get; internal set; }

        /// <summary>
        /// Specifies a collection of variable criteria that is required to unlock this <see cref="ItemSealData"/>.
        /// </summary>
        [JsonProperty("to_unlock")]
        public Dictionary<string, VarProgress> ToUnlock { get; internal set; }
    }
}
