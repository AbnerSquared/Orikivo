using System.Collections.Generic;

namespace Orikivo
{
    // TODO: Create the concepts of game tasks here.
    public class GameTaskProperties
    {
        public List<GameUpdatePacket> OnEntry { get; set; }
        public List<GameAttribute> Attributes { get; set; }
        public List<GameAttribute> UserAttributes { get; set; }
        public List<GameTrigger> Commands { get; set; }
        public List<TaskCompletionPacket> CompletionPackets { get; set; }

        public TaskTimeoutPacket TimeoutPacket { get; set; }
        public string EntryTabId { get; set; }
    }

    // create a function determining what to do upon starting from a specific task.
}
