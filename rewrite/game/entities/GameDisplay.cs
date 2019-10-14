using System.Text;

namespace Orikivo
{
    // container for display content.
    public class GameDisplay
    {
        internal GameDisplay(GameState type)
        {
            Type = type;
            Content = new StringBuilder();
            SyncKey = KeyBuilder.Generate(6);
        }
        // defines what display is correlated to what game state it's meant for.
        //public GameChannel Type { get; }
        public GameState Type { get; }
        public StringBuilder Content { get; private set; }
        public string SyncKey { get; private set; }

        public void Append(string content)
        {
            Content.AppendLine(content);
            SyncKey = KeyBuilder.Generate(6);
        }

        public override string ToString()
        {
            return Content.ToString();
        }
    }
}
