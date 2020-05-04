using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a pre-generated conversation map based on a character.
    /// </summary>
    public class DialogMap
    {
        //
        public string Greeting { get; set; }
        
        public string Farewell { get; set; }
        
        public string OnTimeout { get; set; }


    }

    /// <summary>
    /// Represents a conversation pool, usually for an <see cref="Npc"/> or <see cref="Vendor"/>.
    /// </summary>
    public class DialoguePool // A 
    {
        /// <summary>
        /// Represents the generic response on the start of a conversation.
        /// </summary>
        public string Entry { get; set; }

        /// <summary>
        /// Represents the generic response on the end of a conversation.
        /// </summary>
        public string Exit { get; set; }

        /// <summary>
        /// Represents the generic response on a conversation timeout.
        /// </summary>
        public string Timeout { get; set; }

        /// <summary>
        /// Determines if this <see cref="DialoguePool"/> can be randomly chosen or not.
        /// </summary>
        public bool Generic { get; set; }

        public List<Dialogue> Dialogue { get; set; }

        public List<Dialogue> GetEntryTopics()
            => Dialogue.Where(x => x.Type == DialogType.Initial).ToList();

        public Dialogue GetDialogue(string id)
        {
            if (!Dialogue.Any(x => x.Id == id))
                throw new ArgumentException($"Could not find any dialogue with an ID of '{id}'.");

            return Dialogue.First(x => x.Id == id);
        }
    }

}
