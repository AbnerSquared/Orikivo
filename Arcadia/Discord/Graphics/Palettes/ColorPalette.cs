using System;
using Newtonsoft.Json;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Represents a color palette for a card.
    /// </summary>
    public class ColorPalette : IEquatable<ColorPalette>
    {
        public ColorPalette(PaletteType primary)
        {
            Primary = primary;
            Secondary = null;
        }

        public ColorPalette(PaletteType primary, PaletteType secondary, PaletteMixMethod method = PaletteMixMethod.Smear)
        {
            Primary = primary;
            Secondary = secondary;
            Method = method;
        }

        [JsonConstructor]
        internal ColorPalette(PaletteType primary, PaletteType? secondary, PaletteMixMethod method)
        {
            Primary = primary;
            Secondary = secondary;
            Method = method;
        }

        /// <summary>
        /// Gets the primary <see cref="PaletteType"/> used for this <see cref="ColorPalette"/>.
        /// </summary>
        [JsonProperty("primary")]
        public PaletteType Primary { get; internal set; }

        /// <summary>
        /// Gets the secondary <see cref="PaletteType"/> used for this <see cref="ColorPalette"/>, if one is specified.
        /// </summary>
        [JsonProperty("secondary")]
        public PaletteType? Secondary { get; internal set; }

        /// <summary>
        /// Gets the mixing method of this <see cref="ColorPalette"/>.
        /// </summary>
        [JsonProperty("method")]
        public PaletteMixMethod Method { get; internal set; } = PaletteMixMethod.Smear;

        public GammaPalette Build()
        {
            if (Secondary.HasValue)
            {
                return Method switch
                {
                    PaletteMixMethod.Blend => GammaPalette.Merge(GraphicsService.GetPalette(Primary), GraphicsService.GetPalette(Secondary.Value), 0.5f),
                    _ => GammaPalette.Smear(GraphicsService.GetPalette(Primary), GraphicsService.GetPalette(Secondary.Value))
                };
            }

            return GraphicsService.GetPalette(Primary);
        }

        public static implicit operator ColorPalette(PaletteType primary)
            => new ColorPalette(primary);

        public static implicit operator GammaPalette(ColorPalette palette)
            => palette.Build();

        public static bool operator ==(ColorPalette left, ColorPalette right)
        {
            return (left?.Primary == right?.Primary)
                   & (left?.Secondary == right?.Secondary);
        }

        public static bool operator !=(ColorPalette left, ColorPalette right)
        {
            return (left?.Primary != right?.Primary)
                   | (left?.Secondary != right?.Secondary);
        }

        /// <inheritdoc />
        public bool Equals(ColorPalette other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Primary == other.Primary && Secondary == other.Secondary && Method == other.Method;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((ColorPalette)obj);
        }

        // It might be better to make the hash codes work from read-only properties
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine((int)Primary, (int)(Secondary ?? 0), (int)Method);
        }

        public override string ToString()
        {
            if (Secondary.HasValue)
                return $"{Primary}/{Secondary} ({Method})";

            return $"{Primary}";
        }
    }
}