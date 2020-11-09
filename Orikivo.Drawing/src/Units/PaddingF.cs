namespace Orikivo.Drawing
{
    public readonly struct PaddingF
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

        public float Left { get; }

        public float Right { get; }

        public float Top { get; }

        public float Bottom { get; }

        public float Width => Right + Left;

        public float Height => Bottom + Top;

        public bool IsEmpty =>
            Left == 0
            && Right == 0
            && Bottom == 0
            && Right == 0;
    }
}
