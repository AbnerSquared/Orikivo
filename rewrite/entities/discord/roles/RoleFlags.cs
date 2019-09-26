namespace Orikivo
{
    public class RoleFlags
    {
        public RoleFlags(bool isHoisted = false, bool isManaged = false, bool isMentionable = false)
        {
            IsHoisted = isHoisted;
            IsManaged = isManaged;
            IsMentionable = isMentionable;
        }

        public bool IsHoisted { get; }
        public bool IsMentionable { get; }
        public bool IsManaged { get; }
    }
}