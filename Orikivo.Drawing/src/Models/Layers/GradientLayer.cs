using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Orikivo.Drawing
{
    public class GradientLayer : DrawableLayer
    {
        public Dictionary<float, GammaColor> Markers { get; set; } = new Dictionary<float, GammaColor>();

        public GradientColorHandling ColorHandling { get; set; } = GradientColorHandling.Blend;
        public int Width { get; set; }
        public int Height { get; set; }
        public AngleF Angle { get; set; } = AngleF.Right;

        private GammaColor GetFirstMarker()
        {
            return Markers.OrderBy(x => x.Key).First().Value;
        }

        private GammaColor GetColorAtPoint(float point, GradientColorHandling colorHandling = GradientColorHandling.Blend)
        {
            IEnumerable<float> last = Markers.Keys.Where(x => x < point);
            float? lastClosest = last.Count() > 0 ? last
                .OrderBy(x => MathF.Abs(point - x)).First() : (float?) null;

            IEnumerable<float> next = Markers.Keys.Where(x => x >= point);
            float? nextClosest = next.Count() > 0 ? next
                .OrderBy(x => MathF.Abs(point - x)).First() : (float?) null;

            if (lastClosest == null && nextClosest == null)
                throw new Exception("There are no markers specified within the gradient to draw.");

            if (lastClosest == null && nextClosest != null)
                return Markers[nextClosest.Value];

            if (lastClosest != null && nextClosest == null)
                return Markers[lastClosest.Value];

            GammaColor lastColor = Markers[lastClosest.Value];
            GammaColor nextColor = Markers[nextClosest.Value];

            float strength = RangeF.Convert(lastClosest.Value, nextClosest.Value, 0.0f, 1.0f, point);

            GammaColor result = GammaColor.Merge(lastColor, nextColor, strength);
            //Console.WriteLine($"Merge ({strength}): {result.A}, {result.R}, {result.G}, {result.B}");
            return result;
        }

        private Grid<Color> GetPixels()
        {
            Grid<Color> pixels = new Grid<Color>(Width, Height, GetFirstMarker());

            // for now, this is assumed filling in the right direction.
            for (int i = 0; i < Width; i++)
            {
                // places the point on a 0.0 to 1.0 scale
                float point = RangeF.Convert(0, Width, 0.0f, 1.0f, i);

                pixels.SetColumn(i, GetColorAtPoint(point));

                //Color f = pixels.GetColumn(i)[0];
                //Console.WriteLine($"Column :: {f.A}, {f.R}, {f.G}, {f.B}");
            }

            return pixels;
            // TODO: Implement angle
        }

        protected override Bitmap GetBaseImage()
            => GraphicsUtils.CreateRgbBitmap(GetPixels().Values);
    }
}
