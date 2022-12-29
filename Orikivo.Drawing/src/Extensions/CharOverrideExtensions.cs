namespace Orikivo.Drawing
{
    public static class CharOverrideExtensions
    {
        public static System.Drawing.Point? GetOffset(this CharOverride o)
            => (o.OffsetX.HasValue || o.OffsetY.HasValue)
            ? (System.Drawing.Point?) new System.Drawing.Point(o.OffsetX.GetValueOrDefault(0), o.OffsetY.GetValueOrDefault(0)) : null;
    }
}
