using System;

namespace Arcadia
{
    public interface IInput
    {
        public KeyType Type { get; }
        // what does this key do whenever it is executed?
        public Func<Player, ServerConnection, GameServer, bool> Criterion { get; set; }
        public Action<Player, ServerConnection, GameServer> OnExecute { get; set; }

        public InputResult TryParse(Input input);
    }
}
