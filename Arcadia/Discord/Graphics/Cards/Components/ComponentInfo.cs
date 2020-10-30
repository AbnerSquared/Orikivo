using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Represents the details of how an <see cref="ICardComponent"/> should render.
    /// </summary>
    public class ComponentInfo
    {
        public CardComponent Group { get; set; }
        public ComponentType Type { get; set; }

        // This marks this component to be the primary target for detail inheritance
        // If a primary target is marked as true, and invalid grouping and types were specified, an error would be thrown.
        // Valid targets:
        // - Text/Username
        public bool PrimaryTarget { get; set; }

        public int Priority { get; set; } = 0;
        public int MaxWidth { get; set; } = -1;
        public int MaxHeight { get; set; } = -1;
        public SizeHandling SizeHandling { get; set; } = SizeHandling.Ignore;
        public Padding Padding { get; set; } = Padding.Empty;
        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public CursorOffset CursorOffset { get; set; } = CursorOffset.None;
        public OffsetUsage OffsetUsage { get; set; } = OffsetUsage.Temporary;
        public OffsetHandling OffsetHandling { get; set; } = OffsetHandling.Additive;

        public SizeInherit PreviousInherit { get; set; } = SizeInherit.None;
        public SizeInherit PreviousOffsetInherit { get; set; } = SizeInherit.None;
    }
}