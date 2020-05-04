using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class ChatLog
    {
        // a list of dialog used by the player
        public List<string> Spoken { get; set; }

        // a list of the dialog heard from an npc
        public List<string> Heard { get; set; }
    }

}
