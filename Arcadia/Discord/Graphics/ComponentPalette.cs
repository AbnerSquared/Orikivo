using Newtonsoft.Json;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Represents a customized <see cref="GammaPalette"/>.
    /// </summary>
    public class ComponentPalette
    {
        public ComponentPalette(PaletteType primary)
        {
            Primary = primary;
            Secondary = null;
        }

        public ComponentPalette(PaletteType primary, PaletteType secondary, PaletteMixMethod method = PaletteMixMethod.Smear)
        {
            Primary = primary;
            Secondary = secondary;
            Method = method;
        }

        [JsonConstructor]
        internal ComponentPalette(PaletteType primary, PaletteType? secondary, PaletteMixMethod method)
        {
            Primary = primary;
            Secondary = secondary;
            Method = method;
        }

        /// <summary>
        /// Gets the primary <see cref="PaletteType"/> used for this <see cref="ComponentPalette"/>.
        /// </summary>
        [JsonProperty("primary")]
        public PaletteType Primary { get; internal set; }

        /// <summary>
        /// Gets the secondary <see cref="PaletteType"/> used for this <see cref="ComponentPalette"/>, if one is specified.
        /// </summary>
        [JsonProperty("secondary")]
        public PaletteType? Secondary { get; internal set; }

        /// <summary>
        /// Gets the mixing method of this <see cref="ComponentPalette"/>.
        /// </summary>
        [JsonProperty("method")]
        public PaletteMixMethod Method { get; internal set; } = PaletteMixMethod.Smear;

        public GammaPalette Build()
        {
            if (Secondary.HasValue)
            {
                return Method switch
                {
                    PaletteMixMethod.Merge => GammaPalette.Merge(GraphicsService.GetPalette(Primary), GraphicsService.GetPalette(Secondary.Value), 0.5f),
                    _ => GammaPalette.Smear(GraphicsService.GetPalette(Primary), GraphicsService.GetPalette(Secondary.Value))
                };
            }

            return GraphicsService.GetPalette(Primary);
        }

        public static implicit operator ComponentPalette(PaletteType primary)
            => new ComponentPalette(primary);

        public static implicit operator GammaPalette(ComponentPalette palette)
            => palette.Build();

        public static bool operator ==(ComponentPalette left, ComponentPalette right)
        {
            return (left?.Primary == right?.Primary)
                   & (left?.Secondary == right?.Secondary);
        }

        public static bool operator !=(ComponentPalette left, ComponentPalette right)
        {
            return (left?.Primary != right?.Primary)
                   | (left?.Secondary != right?.Secondary);
        }

        public override string ToString()
        {
            if (Secondary.HasValue)
                return $"{Primary}/{Secondary} ({Method})";

            return $"{Primary}";
        }
    }
}