using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // this could just manage the users playing overall.
    public class GameLobby
    {

        // exposes methods required to check users.
        //private BaseSocketClient _client;
        private ReceiverConfig _defaultReceiverConfig; // used for each new receiver added.
        private GameEventHandler _eventHandler; // what calls and stores events
        public GameLobby(/*BaseSocketClient client,*/ LobbyConfig config, GameEventHandler eventHandler)
        {
            //_client = client;
            _defaultReceiverConfig = config.ReceiverConfig;
            _eventHandler = eventHandler;
            Name = config.Name;
            Privacy = config.Privacy;
            Password = config.Password;
            Mode = config.Mode;

            IsInitialized = false;
        }

        public const int DefaultUserLimit = 16;
        public string Name { get; private set; } // should be limited to the name of a text-channel.
        public LobbyPrivacy Privacy { get; private set; }
        private string Password { get; set; } // if used, will prevent users from joining. In order to join, a DM is sent to the user asking for the password.
        public bool IsProtected => !string.IsNullOrWhiteSpace(Password);
        private bool IsInitialized; // this determines if the lobby has officially started; otherwise, it's considered backline until it starts.

        public GameMode Mode { get; private set; }

        private GameCriteria Criteria => GameCriteria.FromGame(Mode);

        public User Host => Users.FirstOrDefault(x => x.IsHost);
        public List<User> Users { get; private set; } = new List<User>();
        public List<Receiver> Receivers { get; private set; } = new List<Receiver>();
        public int UserCount => Users?.Count ?? 0; // the amount of users currently in the lobby.
        public int UserLimit => Criteria?.UserLimit ?? DefaultUserLimit; // the amount of users that can join before the lobby is full.

        //internal async Task OnDisplayUpdatedAsync(Display previous, Display current, Game game)
        //    => Receivers.ForEach(async x => await x.UpdateAsync(_client, current));

        // this is what is used to start the lobby.
        internal async Task StartAsync(OriCommandContext context)
        {
            if (IsInitialized)
                throw new Exception("The lobby has already started.");

            await AddReceiverAsync(context.Guild);
            User user = new User(context.Account, context.Guild.Id);
            user.Tags = UserTag.Host;
            await AddUserAsync(user);
            IsInitialized = true;
            // add ReceiverChannel info in the receiver.
            // send channel updates ONLY if there's a receiver linked to that
            // display

        }

        // this is used from the GameManager.
        // this allows easy imports
        internal async Task AddUserAsync(OriUser user, SocketGuild guild)
        {
            await AddUserAsync(new User(user, guild.Id));
            await AddReceiverAsync(guild); // make this return softly instead
            // of throwing an exception
        }

        // adds a root user; usually an internal handler.
        internal async Task AddUserAsync(User user)
        {
            if (UserCount == UserLimit)
                throw new Exception("The lobby is full.");
            if (Users.Contains(user))
                throw new Exception("This user is already in the lobby.");
            if (Host != null) // catch duplicate host handling.
                if (user.IsHost)
                    user.Tags = UserTag.None;

            // get all receiver ids that the user can see.
            Users.Add(user);
            await _eventHandler.InvokeUserJoinedAsync(user, this);
            // when the user joins, do this.
        }

        internal async Task RemoveUserAsync(User user) // ulong userId
        {
            // if the user is not in this lobby
            if (!Users.Contains(user))
                throw new Exception("This user is not in the lobby.");

            // if the user about to be removed is the only person in the guild.
            // this handles closing the game entire in this case.
            // _eventHandler.InvokeGameEmptyAsync(); // this triggers what to do when the game is empty AND it has been initialized.


            Users.Remove(user);

            // if the user removed was the host
            if (user.IsHost)
                Users.OrderBy(x => x.JoinedAt).First().Tags = UserTag.Host;
            
            foreach (ulong guildId in user.GuildIds)
                if (!Users.Any(x => x.GuildIds.Contains(guildId)))
                    await RemoveReceiverAsync(guildId);


            await _eventHandler.InvokeUserLeftAsync(user, this);
        }

        internal async Task AddReceiverAsync(SocketGuild guild)
        {
            // you could possibly prevent linking if the guild is already linked to a lobby.
            if (Receivers.Any(x => x.Id == guild.Id))
                throw new Exception("This guild is already linked to this lobby.");

            // maybe handle if there are any users linked to this receiver in the first place?

            Receiver receiver = new Receiver(guild, _defaultReceiverConfig);
            Receivers.Add(receiver);
            await _eventHandler.InvokeReceiverConnectedAsync(receiver, this);
        }

        // removes a receiver.
        internal async Task RemoveReceiverAsync(ulong guildId)
        {
            if (!Receivers.Any(x => x.Id == guildId))
                throw new Exception("There is no receiver that references that guild.");

            Receiver receiver = Receivers.First(x => x.Id == guildId);
            // from what i know, configure await is essentially do the stuff
            // but we don't care about the result.
            await receiver.CloseAsync("This receiver has been closed.", TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            Receivers.Remove(receiver);

            await _eventHandler.InvokeReceiverDisconnectedAsync(receiver, this);
        }

        // only used for closereceiver()
        // removes any direct reference to a guild.
        // this means all users that are directed to this receiver only
        // and the receiver itself.
        internal async Task RemoveGuildAsync(ulong guildId)
        {
            if (!Receivers.Any(x => x.Id == guildId))
                throw new Exception("There is no receiver that references that guild.");

            Receiver receiver = Receivers.First(x => x.Id == guildId);
            await receiver.CloseAsync("This receiver has been closed.", TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            Receivers.Remove(receiver);

            IEnumerable<User> users = Users.Where(x => x.GuildIds.Where(y => y != guildId).Count() == 0);

            foreach (User user in users)
                await RemoveUserAsync(user);

            await _eventHandler.InvokeReceiverDisconnectedAsync(receiver, this);

        }
    }
}
