using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    public class Dialog
    {
        // if left null, it will instead be referenced by BranchId#index
        public string Id { get; set; }

        // determines how a conversation continues.
        public DialogueType Type { get; set; }

        // handles how an NPC will respond and appear
        public DialogueTone Tone { get; set; }

        public DialogEntry Entry => Entries.First();
        // only one of these nodes are randomly chosen
        public List<DialogEntry> Entries { get; set; }
    }

}
