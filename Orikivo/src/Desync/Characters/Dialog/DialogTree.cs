using System.Collections.Generic;

namespace Orikivo.Desync
{
    // make sure that dialog branches can't be repeated, and if there are no new topics,
    // prevent communication with the NPC.
    public class DialogTree
    {
        public string Id { get; set; }

        // this represents all of the possible dialog routes that can be taken
        // each branch has a collection of starting points (or maybe only 1) for the user to select
        // if a branch has more than 1 available starting nodes, list them all out
        // otherwise, just list the single starting node.
        public List<DialogBranch> Branches { get; set; }

        // Represents what will be spoken if the chat handler times out.
        public string OnTimeout { get; set; }

        // what is spoken if the NPC cannot talk right now
        // (if the player exhausted all possible dialog or if the player is interfering with a routine)
        public string OnUnavailable { get; set; }

        // determines if this dialog can be randomly selected or not.
        public bool IsGeneric { get; set; }
    }

}
