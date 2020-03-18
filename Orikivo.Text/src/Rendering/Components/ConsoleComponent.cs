using System;

namespace Orikivo.Text.Rendering
{
    // allow Action<TextEntity> OnCollide2D();

    public class ConsoleComponent : Component
    {
        ConsoleColor? BackgroundColor; // if applied to the console, the background color of the object is set to this, if any
        ConsoleColor? ForegroundColor; // if applied to the console, the foreground color of the object in the console is set, if any
    }
}
