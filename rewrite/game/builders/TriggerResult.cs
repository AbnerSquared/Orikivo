using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// A result deriving from an executed GameTrigger.
    /// </summary>
    public class TriggerResult
    {
        internal TriggerResult() { }

        /// <summary>
        /// The name of the trigger that was parsed.
        /// </summary>
        public string TriggerName { get; internal set; }

        /// <summary>
        /// The object that is obtained from an argument passed. Can be null.
        /// </summary>
        public object Result { get; internal set; }

        /// <summary>
        /// An collection of attributes to update.
        /// </summary>
        public List<AttributeUpdatePacket> Packets { get; internal set; }

    }
}
