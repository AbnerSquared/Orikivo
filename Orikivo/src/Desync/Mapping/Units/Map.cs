using Orikivo.Drawing;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a visual display of a <see cref="Location"/> in a <see cref="World"/>.
    /// </summary>
    public class Map
    {
        public Map(Sprite source, byte[] progression)
        {
            // THE MORE YOU KNOW
            Source = source ?? throw new System.NullReferenceException("The source image referenced is null.");
            Progression = Engine.DecompressMap(source.Width, source.Height, progression);
        }

        /// <summary>
        /// Represents the source of this <see cref="Map"/>.
        /// </summary>
        public Sprite Source { get; set; }

        /// <summary>
        /// Represents the completion of this <see cref="Map"/>.
        /// </summary>
        public Grid<bool> Progression { get; set; }

        public List<Indicator> Indicators { get; set; }

        // TODO: Create a method that updates the progression map based on a view radius and position.
        public void Update() => throw new System.NotImplementedException();

        // Compresses the progression map back into its raw bit counterpart.
        public byte[] Compress()
            => Engine.CompressMap(Progression);
    }
}
