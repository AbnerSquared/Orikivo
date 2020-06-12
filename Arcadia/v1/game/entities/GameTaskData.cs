using System.Collections.Generic;
using System.Linq;

namespace Arcadia.Old
{
    public class GameTaskData
    {
        public GameTaskData(string taskId, List<GameAttribute> attributes = null, List<GameTrigger> triggers = null)
        {
            TaskId = taskId;
            TaskAttributes = attributes ?? new List<GameAttribute>();
            TaskTriggers = triggers ?? new List<GameTrigger>();
        }

        // the game client data.
        public GameClientData Root { get; internal set; }
        public string TaskId { get; }
        public List<Player> Players => Root?.Players;
        public List<GameAttribute> TaskAttributes { get; } = new List<GameAttribute>();
        public List<GameTrigger> TaskTriggers { get; } = new List<GameTrigger>();
        public List<GameAttribute> Attributes => TaskAttributes.Concat(Root?.Attributes ?? new List<GameAttribute>()).ToList();
        public List<GameTrigger> Triggers => TaskTriggers.Concat(Root?.Triggers ?? new List<GameTrigger>()).ToList();
    }
}
