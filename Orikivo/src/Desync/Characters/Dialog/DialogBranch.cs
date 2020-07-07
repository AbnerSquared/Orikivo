using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Desync
{
    public class DialogBranch
    {
        public string Id { get; set; }

        // determines who starts the conversation (NPC or USER)
        public DialogSpeaker StartingSpeaker { get; set; }

        public List<Dialog> Dialogs { get; set; }

        public Dialog GetDialog(string id)
        {
            return Dialogs.First(x => x.Id == id);
        }

        // gets the list of starting dialogs to choose from based on the npc and player.
        public IEnumerable<Dialog> GetEntryDialogs(Character npc, Husk husk, HuskBrain brain)
        {
            switch (StartingSpeaker)
            {
                case DialogSpeaker.Npc:
                    return Dialogs.Where(x => x.Speaker == DialogSpeaker.Npc && x.Type == DialogType.Initial);

                default:
                    return Dialogs.Where(x => x.Speaker == DialogSpeaker.User && x.Type == DialogType.Initial);
            }
        }

        // gets the best reply for a dialog based on the criteria met for an NPC.
        public Dialog GetBestReply(Character npc, Husk husk, HuskBrain brain)
        {
            throw new NotImplementedException();
        }

        // gets a collection of available replies for the player to use based
        // on the criteria met.
        public List<Dialog> GetAvailableReplys(Character npc, Husk husk, HuskBrain brain)
        {
            throw new NotImplementedException();
        }
    }

}
