using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{

    /// <summary>
    /// Represents a conversation node, usually for an <see cref="Npc"/> or <see cref="Vendor"/>.
    /// </summary>
    public class Dialogue
    {
        /// <summary>
        /// A unique identifier for this <see cref="Dialogue"/>.
        /// </summary>
        public string Id { get; set; }

        public string Entry => Entries.First();

        /// <summary>
        /// Represents a collection of sentences that can be used when this <see cref="Dialogue"/> is called. The first value specified is considered the main entry.
        /// </summary>
        public List<string> Entries { get; set; }

        // TODO: Implement auto duration
        public TimeSpan AutoDuration { get; set; }

        /// <summary>
        /// Determines how this <see cref="Dialogue"/> is handled.
        /// </summary>
        public DialogueType Type { get; set; }

        /// <summary>
        /// Determines how an <see cref="Npc"/> receives this <see cref="Dialogue"/> (user-side) or how an <see cref="Npc"/> will look when speaking (client-side).
        /// </summary>
        public DialogueTone Tone { get; set; }

        /// <summary>
        /// Determines how strong this <see cref="Dialogue"/> affects an <see cref="Npc"/>.
        /// </summary>
        public float Importance { get; set; }

        /// <summary>
        /// A collection of possible <see cref="Dialogue"/> values that can be used in response to this <see cref="Dialogue"/>.
        /// </summary>
        public List<string> ReplyIds { get; set; }

        // if none is set, ignore.
        public DialogueCriterion Criterion { get; set; }

        /// <summary>
        /// Returns the best matching reply ID based on a <see cref="Personality"/> and <see cref="Relationship"/>.
        /// </summary>
        public string GetBestReplyId(Archetype personality)
        {
            // TODO: Create a system for choosing best replies.
            return Randomizer.Choose(ReplyIds);
        }

        /// <summary>
        /// Returns a random <see cref="string"/> that represents what will be spoken.
        /// </summary>
        public string NextEntry()
            => Randomizer.Choose(Entries);
    }

}
