namespace Orikivo.Text.Rendering
{
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
}
