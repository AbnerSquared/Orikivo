using System;

namespace Arcadia

{
    // an action
    public class TextInput : IInput
    {
        // what does the player need to type to execute this key?
        public string Name { get; set; }

        public KeyType Type => KeyType.Text;

        public Func<Player, bool> Criterion { get; set; }

        public Action<Player, GameServer> OnExecute { get; set; }
    }
}
