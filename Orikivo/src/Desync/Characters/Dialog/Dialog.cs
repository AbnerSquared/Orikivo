using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    public class Dialog
    {
        // if left null, it will instead be referenced by BranchId#index
        public string Id { get; set; }

        // this determines if this exact dialog can be repeated.
        public bool CanRepeat { get; set; }

        // determines who uses this dialog
        // this is used to help when randomly selecting choices.
        public DialogSpeaker Speaker { get; set; }

        // determines how a conversation continues.
        public DialogType Type { get; set; }

        // handles how an NPC will respond and appear
        public DialogTone Tone { get; set; }

        // the first entry is always used by the player
        public DialogEntry Entry => Entries.First();

        // only one of these nodes are chosen if an npc is using this dialog.
        // entries should always convey the same message, just spoken in a
        // different style, depending on the NPC's personality.
        public List<DialogEntry> Entries { get; set; }

        // this is a list of dialog IDs that can be used in response to this dialog.
        public List<string> ReplyIds { get; set; }

        // the criterion in order for this dialog to be used.
        // let's say a dialog's reply ids point to another dialog
        // to determine which dialog we choose, we compare criterion and priority
        // if there are multiple successful criterions with the same priority
        // one is chosen at random.
        public DialogCriterion ToUse { get; set; }

        // this is executed at the end of this dialog.
        public DialogResult Result { get; set; }


        // gets the best DialogEntry based on an NPC.
        public DialogEntry GetBestEntry(Npc npc)
        {
            DialogEntry result = null;

            foreach (var entries in Entries.Where(x => x.Criterion != null)
                .GroupBy(x => x.Criterion.Priority).OrderByDescending(x => x.Key))
            {
                var available = entries.Where(x => x.Criterion.Judge?.Invoke(npc) ?? false);

                if (available.Count() == 0)
                {
                    continue;
                }
                else
                {
                    result = Randomizer.Choose(available);
                    break;
                }
            }

            if (result == null)
            {
                var generic = Entries.Where(x => x.Criterion == null);

                if (generic.Count() == 0)
                {
                    throw new System.Exception("Could not find any available entries.");
                }

                return Randomizer.Choose(generic);
            }

            return result;
        }
    }
}
