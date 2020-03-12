using System.Drawing;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Stream = System.IO.Stream;

namespace Orikivo.Drawing.Encoding
{
    public static class EncodeUtils
    {
        public static void CreateGifStream(Image image, Stream stream, Quality quality = Quality.Bpp8)
        {
            if (quality == Quality.Inherit)
                image.Save(stream, ImageFormat.Gif);
            else
            {
                Quantizer quantizer = quality switch
                {
                    Quality.Grayscale => new GrayscaleQuantizer(),
                    Quality.Bpp1 => new OctreeQuantizer(1, 1),
                    Quality.Bpp2 => new OctreeQuantizer(3, 2),
                    Quality.Bpp4 => new OctreeQuantizer(15, 4),
                    Quality.Bpp8 => new OctreeQuantizer(255, 8),
                    _ => new OctreeQuantizer(255, 8)
                };

                using (Bitmap result = quantizer.Quantize(image))
                    result.Save(stream, ImageFormat.Gif);
            }
        }

        public static Image Quantize(Image image, Quality quality = Quality.Bpp8)
        {
            if (quality == Quality.Inherit)
                return image;

            Quantizer quantizer = quality switch
            {
                Quality.Grayscale => new GrayscaleQuantizer(),
                Quality.Bpp1 => new OctreeQuantizer(1, 1),
                Quality.Bpp2 => new OctreeQuantizer(3, 2),
                Quality.Bpp4 => new OctreeQuantizer(15, 4),
                Quality.Bpp8 => new OctreeQuantizer(255, 8),
                _ => new OctreeQuantizer(255, 8)
            };

            return quantizer.Quantize(image);
        }
    }
}
