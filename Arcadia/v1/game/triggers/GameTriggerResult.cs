using System.Collections.Generic;

namespace Arcadia.Old
{
    /// <summary>
    /// A result deriving from an executed GameTrigger.
    /// </summary>
    public class GameTriggerResult
    {
        internal GameTriggerResult() { }

        /// <summary>
        /// The name of the trigger that was parsed.
        /// </summary>
        public string TriggerId { get; internal set; }

        // The id of an argument that was passed.
        public string ArgId { get; internal set; }

        /// <summary>
        /// The objects that were obtained from an argument passed. Can be empty.
        /// </summary>
        public List<GameObject> Objects { get; internal set; } = new List<GameObject>();

        /// <summary>
        /// Packets to be updated within a game.
        /// </summary>
        public List<GameUpdatePacket> Packets { get; internal set; } = new List<GameUpdatePacket>();

        public bool IsSuccess => !Error.HasValue;
        public TriggerParseError? Error { get; internal set; }
        public string ErrorReason { get; internal set; }
    }
}
