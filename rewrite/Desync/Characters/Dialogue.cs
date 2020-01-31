using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Unstable
{
    // TODO: Apply DialogueTone
    // TODO: Apply Importance, which affects how strongly it alters a relationship.
    // TODO: Implement Personality determining for Getting the best possible response.
    /// <summary>
    /// Represents a conversation node, usually for an <see cref="Npc"/> or <see cref="Vendor"/>.
    /// </summary>
    public class Dialogue
    {
        public string Id { get; set; }

        public string Entry => Entries.First();
        public List<string> Entries { get; set; }

        public DialogueType Type { get; set; }

        // This is used both to determine how an NPC will handle the dialogue AND how an NPC will look when speaking the dialogue.
        public DialogueTone Tone { get; set; }

        // a list of possible responses.
        public List<string> ReplyIds { get; set; }

        // gets the best reply that fits with the NPC's personality.
        public string GetBestReplyId(Archetype personality)
        {
            // for now, just get a random reply.
            return Randomizer.Choose(ReplyIds);
        }

        public string NextEntry()
            => Randomizer.Choose(Entries);
    }

}
