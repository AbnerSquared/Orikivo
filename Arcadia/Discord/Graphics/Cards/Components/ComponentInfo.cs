using Newtonsoft.Json;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Represents information about how a <see cref="CardComponent"/> is rendered.
    /// </summary>
    public class ComponentInfo
    {
        /// <summary>
        /// Defines the card grouping of this component.
        /// </summary>
        [JsonProperty("group")]
        public CardGroup Group { get; set; }

        /// <summary>
        /// Defines the base type of this component.
        /// </summary>
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        /// <summary>
        /// Determines if this component is a target for property inheritance.
        /// </summary>
        [JsonProperty("primary_target")]
        public bool PrimaryTarget { get; set; }

        /// <summary>
        /// Determines the rendering position order of this component (ascending).
        /// </summary>
        [JsonProperty("position")]
        public int Position { get; set; }

        /// <summary>
        /// Defines the initial height of this component.
        /// </summary>
        [JsonProperty("base_height")]
        public int BaseHeight { get; set; }

        /// <summary>
        /// Defines the initial width of this component.
        /// </summary>
        [JsonProperty("base_width")]
        public int BaseWidth { get; set; }

        /// <summary>
        /// Defines the maximum width of this component.
        /// </summary>
        [JsonProperty("max_width")]
        public int MaxWidth { get; set; } = -1;

        /// <summary>
        /// Defines the maximum height of this component.
        /// </summary>
        [JsonProperty("max_height")]
        public int MaxHeight { get; set; } = -1;

        /// <summary>
        /// Determines how size specifications are handled for this component.
        /// </summary>
        [JsonProperty("size_handling")]
        public SizeHandling SizeHandling { get; set; } = SizeHandling.Ignore;

        /// <summary>
        /// Defines the default fill information of this component.
        /// </summary>
        [JsonProperty("fill")]
        public BaseFillInfo Fill { get; set; }

        /// <summary>
        /// Defines the default outline fill information of this component.
        /// </summary>
        [JsonProperty("outline")]
        public BaseFillInfo Outline { get; set; }

        /// <summary>
        /// Defines the padding for this component.
        /// </summary>
        [JsonProperty("padding")]
        public Padding Padding { get; set; } = Padding.Empty;

        /// <summary>
        /// Defines the horizontal placement offset for this component during initialization (does not affect the cursor).
        /// </summary>
        [JsonProperty("offset_x")]
        public int OffsetX { get; set; }

        /// <summary>
        /// Defines the vertical placement offset for this component during initialization (does not affect the cursor).
        /// </summary>
        [JsonProperty("offset_y")]
        public int OffsetY { get; set; }

        /// <summary>
        /// Determines what this component applies to the cursor during post-operation.
        /// </summary>
        [JsonProperty("cursor_offset")]
        public CursorOffset CursorOffset { get; set; } = CursorOffset.None;

        /// <summary>
        /// Determines how this component handles offsets during post-operation.
        /// </summary>
        [JsonProperty("offset_finalize")]
        public OffsetFinalize OffsetFinalize { get; set; } = OffsetFinalize.Ignore;

        /// <summary>
        /// Determines how this component initializes offsets.
        /// </summary>
        [JsonProperty("offset_handling")]
        public OffsetHandling OffsetHandling { get; set; } = OffsetHandling.Additive;

        /// <summary>
        /// Determines what this component's size inherits from the previous component.
        /// </summary>
        [JsonProperty("size_inherit")]
        public SizeInherit SizeInherit { get; set; } = SizeInherit.None;

        /// <summary>
        /// Determines what this component's offset inherits from the previous component.
        /// </summary>
        [JsonProperty("offset_inherit")]
        public SizeInherit OffsetInherit { get; set; } = SizeInherit.None;
    }
}
