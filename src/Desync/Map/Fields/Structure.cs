using Orikivo.Drawing;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a simple structure in a <see cref="Field"/> or <see cref="Sector"/>.
    /// </summary>
    public class Structure
    {
        public StructureType Type { get; set; }

        // where the structure is location in relation to the field
        public Vector2 Position { get; set; }
    }
}
