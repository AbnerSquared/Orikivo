using System.Text;

namespace Orikivo
{
    // this is used to keep data across games.
    public class GameAttribute
    {
        public GameAttribute(string id, int defaultValue = 0)
        {
            Id = id;
            Value = DefaultValue = defaultValue;
        }

        public string Id { get; } // werewolf:werewolvesLeft
        public int Value { get; internal set; } // 2
        public int DefaultValue { get; } // 0
    }
}
