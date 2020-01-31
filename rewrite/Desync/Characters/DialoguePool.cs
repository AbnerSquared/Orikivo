using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a conversation pool, usually for an <see cref="Npc"/> or <see cref="Vendor"/>.
    /// </summary>
    public class DialoguePool // A 
    {
        /// <summary>
        /// Represents the initial response on the start of a conversation.
        /// </summary>
        public string Entry { get; set; }

        public List<Dialogue> Dialogue { get; set; }

        public List<Dialogue> GetEntryTopics()
            => Dialogue.Where(x => x.Type == DialogueType.Initial).ToList();

        public Dialogue GetDialogue(string id)
        {
            if (!Dialogue.Any(x => x.Id == id))
                throw new ArgumentException($"No dialogue was found by the ID of '{id}'.");

            return Dialogue.First(x => x.Id == id);
        }
    }

}
