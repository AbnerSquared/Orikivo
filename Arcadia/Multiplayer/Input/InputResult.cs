using System.Collections.Generic;

namespace Arcadia.Multiplayer

{
    public class InputResult
    {
        public bool IsSuccess { get; set; }

        // the input that was successfully parsed
        public IInput Input { get; set; }

        public List<string> Args { get; set; } = new List<string>();
    }
}
