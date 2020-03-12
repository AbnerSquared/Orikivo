using System;
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

    // stores the x- and y-components of velocity for an entity.
    public class Vector
    { }

    public enum ComponentFlag
    {
        Collider = 1,
        Console = 2,
        Rigidbody = 4
    }

    public class Solid : TextEntity
    {
        // Initiate using a basic 0.0 to 1.0 rectangular array
    
    }

    public class TextEntity
    {
        char[,] Shape; // a rectangular array containing the design of the entity
        int Width; // the width of the entity
        int Height; // the height of the entity
        Vector Velocity; // stores the entity's velocity
        Component[] Components; // a list of components

        public virtual void Start() { } // what to do to this entity when it initialized
        public virtual void Update() { } // what to do on each frame update
    }

    public class Camera
    {
        string Shader; // a string containing brightness values from low to high
        int Width; // the viewport width of the camera
        int Height; // the viewport height of the camera
        char Void; // the character used if there is nothing to draw
        char[,] Screen; // this is always applied over the camera's lens.
    }

    public class Scene // a sizable room with objects
    {
        TextEntity[] Objects; // a  collection of 'sprites'
    }

    // allow Action<TextEntity> OnCollide2D();

    public class ConsoleComponent : Component
    {
        ConsoleColor? BackgroundColor; // if applied to the console, the background color of the object is set to this, if any
        ConsoleColor? ForegroundColor; // if applied to the console, the foreground color of the object in the console is set, if any
    }

    public enum BorderFlag
    {
        Stop = 1, // the colliding object is stoppedat the velocity it collided at
        Ignore = 2, // the colliding object is left as is
        Wrap = 3, // any colliding object is pushed to the opposite end
        Invert = 4 // any colliding object is inverted (velocity-wise)
    }

    public class Collider : Component
    {
        int OffsetX; // the starting x point for the collider
        int OffsetY; // the starting y point for the collider
        int Width; // the width of the collider
        int Height; // the height of the collider
    }

    // represents an automatic handling of velocity, acceleration, and gravity.
    public class Rigidbody : Component
    {
        float Gravity; // gravity scale
        float Mass; // the mass of the rigidbody
        bool Active; // is the rigidbody active?
    }

    public class Component
    {
        ComponentFlag Flag; // the type of component that it is
    }
}
