namespace Orikivo
{
    // figure out how to split this info to be a separate variant.
    public class WerewolfUser
    {
        public WerewolfUser(User user)
        {
            Name = user.Name;
            Id = user.Id;
            ReceiverId = user.GuildIds[0];
            _setRole = false;

            Protected = false;
            Scanned = false;
            Dead = false;
        }

        public string Name { get; }
        public ulong Id { get; }
        public ulong ReceiverId { get; }
        private bool _setRole;
        public WerewolfRoleType Role { get; private set; }
        public bool IsWerewolf { get { return Role == WerewolfRoleType.Werewolf; } }
        public bool Protected { get; private set; }
        public bool Scanned { get; private set; }
        public bool Dead { get; private set; }

        public void SetRole(WerewolfRoleType roleType)
        {
            Role = roleType;
            _setRole = true;
        }
    }
}
