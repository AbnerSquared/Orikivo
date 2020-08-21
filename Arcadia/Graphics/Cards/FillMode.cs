namespace Arcadia.Graphics
{
    public enum FillMode
    {
        // REQ: NONE
        None = 0,

        // REQ: Palette, Primary
        Solid = 1,

        // REQ: Palette, Primary, Secondary, Direction
        Experience = 2,

        // REQ: Palette, Direction
        Gradient = 3,

        // REQ: Palette
        Reference = 4
    }
}