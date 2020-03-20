namespace Orikivo.Drawing
{
    public struct UnitF
    {
        public UnitF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public UnitF(float wh)
        {
            Width = Height = wh;
        }
        public float Width { get; set; }
        public float Height { get; set; }

        public bool IsEmpty => Width == 0 && Height == 0;
    }
}
