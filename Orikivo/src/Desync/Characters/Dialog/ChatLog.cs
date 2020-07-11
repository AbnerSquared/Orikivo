using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class ChatLog
    {
        // a list of ids used by the player
        public List<string> Spoken { get; set; } = new List<string>();

        // a list of the ids heard from an npc
        public List<string> Heard { get; set; } = new List<string>();
    }

}
