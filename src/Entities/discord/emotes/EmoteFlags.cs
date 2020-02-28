namespace Orikivo
{
    public class EmoteFlags
    {
        public EmoteFlags(bool isAnimated = false, bool isExclusive = false, bool isManaged = false, bool requireColons = false)
        {
            IsAnimated = isAnimated;
            //IsBoosted = isBoosted;
            IsExclusive = isExclusive;
            IsManaged = isManaged;
            RequireColons = requireColons;
        }

        public bool IsAnimated { get; }
        //public bool IsBoosted { get; }
        public bool IsExclusive { get; }
        public bool IsManaged { get; }
        public bool RequireColons { get; }
    }
}