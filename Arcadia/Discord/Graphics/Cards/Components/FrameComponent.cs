using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public class FrameComponent
    {
        public string ReferencePath;
        public int ReferenceIndex;
        public int CropWidth;
        public int CropHeight;
        public GammaPalette Palette;
        // This defines what this frame is meant for, so that it can't be incorrectly assigned
        public CardComponent AvailableTypes;
    }
}