using System.Collections.Generic;

namespace Orikivo.Desync
{
    // make sure that dialog branches can't be repeated, and if there are no new topics,
    // prevent communication with the NPC.
    public class DialogTree
    {
        public string Id { get; set; }
        // this represents all of the possible dialog routes that can be taken
        public List<DialogBranch> Branches { get; set; }

        // what will be spoken if the chat handler times out.
        public string OnTimeout { get; set; }

        // determines if this dialog can be randomly selected or not.
        public bool IsGeneric { get; set; }
    }

}
