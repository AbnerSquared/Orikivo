using System.Collections.Generic;

namespace Orikivo.Desync
{
    public class ChatLog : ActionLog
    {
        // the ids of everything the user has said up to this point
        public List<string> UserReplyIds { get; set; }

        // the ids of everything the npc has said up to this point
        public List<string> NpcReplyIds { get; set; }
    }

}
