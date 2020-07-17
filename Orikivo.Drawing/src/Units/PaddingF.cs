namespace Orikivo.Drawing
{
    public struct PaddingF
    {
        public PaddingF(float ltrb)
        {
            Left = Right = Top = Bottom = ltrb;
        }

        public PaddingF(float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public float Left { get; set; }

        public float Right { get; set; }

        public float Top { get; set; }

        public float Bottom { get; set; }

        public float Width => Right + Left;

        public float Height => Bottom + Top;
    }
}
