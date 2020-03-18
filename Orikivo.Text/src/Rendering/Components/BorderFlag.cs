namespace Orikivo.Text.Rendering
{
    public enum BorderFlag
    {
        Stop = 1, // the colliding object is stoppedat the velocity it collided at
        Ignore = 2, // the colliding object is left as is
        Wrap = 3, // any colliding object is pushed to the opposite end
        Invert = 4 // any colliding object is inverted (velocity-wise)
    }
}
