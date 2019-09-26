using Discord.WebSocket;

namespace Orikivo
{
    // This is the generic info that role provides, which is the color.
    public class RoleBaseInfo : EntityInfo
    {
        public RoleBaseInfo(SocketRole role) : base(role)
        {
            Color = new OriColor(role.Color.RawValue);
        }

        public OriColor Color { get; }
    }
}