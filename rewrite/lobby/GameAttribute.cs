using System.Text;

namespace Orikivo
{

    // this is used to keep game rules/data stored
    public class GameAttribute
    {
        public GameAttribute(string name, int defaultValue = 0)
        {
            Name = name;
            DefaultValue = defaultValue;
        }
        public string Name { get; } // werewolf:werewolvesLeft
        public int Value { get; set; } // 2
        public int DefaultValue { get; } // 0
    }
}
