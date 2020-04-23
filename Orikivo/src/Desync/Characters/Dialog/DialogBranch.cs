using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class DialogBranch
    {
        public string Id { get; set; }

        public List<Dialog> Dialogs { get; set; }

        // gets the list of starting dialogs to choose from based on the npc and player.
        public List<Dialog> GetEntryDialogs(Npc npc, Husk husk, HuskBrain brain)
        {
            throw new NotImplementedException();
        }

        // gets the best reply for a dialog based on the criteria met for an NPC.
        public Dialog GetBestReply(Npc npc, Husk husk, HuskBrain brain)
        {
            throw new NotImplementedException();
        }

        // gets a collection of available replies for the player to use based
        // on the criteria met.
        public List<Dialog> GetAvailableReplys(Npc npc, Husk husk, HuskBrain brain)
        {
            throw new NotImplementedException();
        }
    }

}
