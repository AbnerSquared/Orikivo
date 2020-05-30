using System;

namespace Arcadia

{
    public interface IInput
    {
        public KeyType Type { get; }
        // what does this key do whenever it is executed?
        public Func<Player, bool> Criterion { get; set; }
        public Action<Player, GameServer> OnExecute { get; set; }
    }
}
