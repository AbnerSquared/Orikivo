namespace Orikivo.Drawing.Graphics3D
{
    internal static class Utils
    {
        public static float Radians(float angle)
        {
            return (angle / 180.0f) * 3.14159274f;
        }

        public static float GetImageRatio(ImageRatio ratio)
        {
            return ratio switch
            {
                ImageRatio.Square => 1.0f,
                ImageRatio.Tall => 0.5f,
                ImageRatio.Wide => 2.0f,
                ImageRatio.Rectangle => 4.0f / 3.0f,
                ImageRatio.Widescreen => 16.0f / 9.0f,
                _ => 1.0f
            };
        }
    }
}
