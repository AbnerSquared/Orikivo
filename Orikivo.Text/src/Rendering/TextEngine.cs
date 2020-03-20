using System.Collections.Generic;
using System.Text;

namespace Orikivo.Text.Rendering
{
    public class TextEngine
    {
        public virtual void Start() { } // what to do on initialization
        public virtual void Update() { } // what to do on each frame update

        public Scene Scene { get; set; } // the main 'room' to render
    }
}
