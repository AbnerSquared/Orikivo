using System.Collections.Generic;
using System.Linq;

namespace Orikivo
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
        public GameData Root { get; internal set; }
        public string TaskId { get; }
        public List<Player> Players => Root?.Players;
        public List<GameAttribute> TaskAttributes { get; }
        public List<GameTrigger> TaskTriggers { get; }
        public List<GameAttribute> Attributes => TaskAttributes.Concat(Root?.Attributes ?? new List<GameAttribute>()).ToList();
        public List<GameTrigger> Triggers => TaskTriggers.Concat(Root?.Triggers ?? new List<GameTrigger>()).ToList();
    }
}
