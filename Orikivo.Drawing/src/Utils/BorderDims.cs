namespace Orikivo.Drawing
{
    internal class BorderDims
    {
        internal int OuterLength { get; set; }

        internal int InnerLength { get; set; }

        internal int MaxInnerLength { get; set; }

        internal int ImageWidth { get; set; }

        internal int ImageHeight { get; set; }

        internal int Thickness => OuterLength + InnerLength;
    }
}
