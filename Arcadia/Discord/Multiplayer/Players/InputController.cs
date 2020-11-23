using System.Collections.Generic;

namespace Arcadia.Multiplayer
{
    public class InputController
    {
        public List<IInput> Inputs { get; set; }

        public bool Disabled { get; set; }
    }
}