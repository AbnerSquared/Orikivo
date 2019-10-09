using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    #pragma warning disable CS1998
    public class GameLobby
    {
        public static int DefaultUserLimit = 16;

        private GameBootCriteria _bootCriteria => GameBootCriteria.FromMode(Mode);
        private GameReceiverConfig _receiverConfig;
        private GameEventHandler _events;

        public GameLobby(GameConfig config, GameEventHandler events)
        {
            _events = events;
        }

        internal bool IsInitialized { get; private set; } = false;
        public string Name { get; private set; }
        public GamePrivacy Privacy { get; private set; }
        public string Password { get; private set; }
        public bool IsProtected => Checks.NotNull(Password);
        public GameMode Mode { get; private set; }
        public List<GameReceiver> Receivers { get; } = new List<GameReceiver>();
        public List<User> Users { get; } = new List<User>();
        public User Host => Users.FirstOrDefault(x => x.IsHost);
        public int UserCount => Users.Count;
        public int UserLimit => _bootCriteria?.UserLimit ?? DefaultUserLimit;

        internal async Task BootAsync(OriCommandContext context)
        {
            if (IsInitialized)
                throw new Exception("The game session has already started.");

            await AddReceiverAsync(context.Guild);
            User user = new User(context.Account.Id, context.Account.Name, context.Guild.Id, UserTag.Host);
            await AddUserAsync(user);
            IsInitialized = true;
        }

        internal async Task AddUserAsync(OriUser user, SocketGuild guild)
        {
            await AddReceiverAsync(guild);
            await AddUserAsync(new User(user.Id, user.Name, guild.Id));
        }

        private async Task AddUserAsync(User user)
        {
            if (UserCount >= UserLimit)
                throw new Exception("The game is full.");
            if (ContainsUser(user.Id))
                throw new Exception("The user is already in this game.");
            if (!ContainsGuild(user.ReceiverId))
                throw new Exception("The user being added doesn't have any receiver to read from.");
            if (Host != null && user.IsHost)
                user.Tags = UserTag.Empty;

            Users.Add(user);
            // await _events.InvokeUserJoinedAsync(user, this);
        }

        internal async Task RemoveUserAsync(ulong userId)
            => await RemoveUserAsync(Users.First(x => x.Id == userId));

        private async Task RemoveUserAsync(User user)
        {
            if (!ContainsUser(user.Id))
                throw new Exception("This user is already not in this game.");
            Users.Remove(user);
            // if UserCount - 1 == 0
            // await CloseAsync();
        }

        internal async Task AddReceiverAsync(SocketGuild guild)
        {
            if (ContainsGuild(guild.Id))
                return; // this isn't too big of a deal; just cancel the task
                // throw new Exception("This guild is already a receiver for this game.");

            GameReceiver receiver = new GameReceiver(guild, _receiverConfig);
            Receivers.Add(receiver);
            // await _events.InvokeReceiverConnectedAsync(receiver, this);
        }

        private async Task RemoveReceiverAsync(GameReceiver receiver)
        {
            if (!ContainsGuild(receiver.Id))
                throw new Exception("This receiver is not in this group.");

            await receiver.CloseAsync();
            Receivers.RemoveAll(x => x.Id == receiver.Id);
            // await _events.InvokeReceiverDisconnectedAsync(receiver, this);
        }

        internal async Task ClearAsync()
        {
            Receivers.ForEach(async x => await x.CloseAsync());
            Receivers.Clear();
            Users.Clear();
        }

        public bool ContainsUser(ulong userId)
            => Users.Any(x => x.Id == userId);
        public bool ContainsGuild(ulong guildId)
            => Receivers.Any(x => x.Id == guildId);

        public bool CanStart => _bootCriteria.Check(UserCount);
    }
}
