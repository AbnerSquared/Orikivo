namespace Arcadia.Graphics
{
    public class SheetInfo
    {
        public string Path { get; set; }

        public int Index { get; set; }

        public int CropWidth { get; set; }

        public int CropHeight { get; set; }

        public bool AutoTrim { get; set; } = true;
    }
}
