namespace Orikivo.Text.Rendering
{
    // represents an automatic handling of velocity, acceleration, and gravity.
    public class Rigidbody : Component
    {
        float Gravity; // gravity scale
        float Mass; // the mass of the rigidbody
        bool Active; // is the rigidbody active?
    }
}
