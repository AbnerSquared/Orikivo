using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public class OriLobbyInvoker
    {
        // these are all of the base requirements for a lobby to function.
        private BaseSocketClient _rootClient;
        private ReceiverConfig _receiverConfig;
        private DisplayConfig _displayConfig;


        // the game mode could be left null, but be unable to start until a game mode is specified.
        public GameMode Mode { get; private set; }
        private GameCriteria Criteria { get { return GameCriteria.FromGame(Mode); } }
        private bool _isGlobal;
        private List<LobbyTrigger<bool>> _triggers;


        // instead of manually typing out each node index
        // we could create a LobbyDisplayMetadata, which would keep track of all nodes and groups.
        private bool _isConfigOpen;
        private bool _isReceiversOpen;
        private int? _localNodeGroup;
        private int _consoleNodeGroup;
        private int _triggerNodeGroup;
        private int _userNodeGroup;
        private int? _triggerCloseNode;
        private int? _triggerHostNode;
        private int? _triggerTitleNode;
        private int? _triggerGameNode;
        private int? _triggerPrivacyNode;
        private int? _triggerReceiverNode;
        private int? _triggerCloseReceiverNode;

        private List<int?> _triggerLocalNodes;

        //public event EventHandler MessageReceived;
        //private DateTime LastMessageTime;
        //private int _messagesSent; // 
        //private int _untilLimitReset; // 5 seconds

        // TimeUntilRateLimitReset;
        public string Id { get; }

        private ulong _localReceiverId;
        
        // last step: Organize, Finalize Triggers, Allow for TriggerArgs, and allow internal game process, passing on the display.
        public OriLobbyInvoker(BaseSocketClient rootClient, OriCommandContext context, ReceiverConfig receiverConfig,
            LobbyConfig config, DisplayConfig displayConfig = null)
        {
            Id = KeyBuilder.Generate(8); // lobby id

            _rootClient = rootClient;
            _receiverConfig = receiverConfig;

            _displayConfig = displayConfig ?? new DisplayConfig();

            Receivers = new List<Receiver>();
            Users = new List<User>();

            Mode = config.Mode;
            _isGlobal = config.Privacy == LobbyPrivacy.Public;

            IsRunning = false;
            IsGameStarted = false;
            Closed = false;

            Console.WriteLine("[Debug] -- Constructing display node. --");
            Display = new Display(_displayConfig);
            Display.SetTitle(null);

            Console.WriteLine("[Debug] -- Building lobby display. --");

            // GROUP 0, CONSOLE
            _consoleNodeGroup = Display.AddGroup(new StringNodeGroup
            {
                Title = config.Name,
                PageValueLimit = 8,
                AllowFormatting = false
            }).Index;

            // group 1: Users
            _userNodeGroup = Display.AddGroup(new StringNodeGroup
            {
                Title = $"Users **{Counter}**",
                TitleMap = null,
                PageValueLimit = null,
                ContentMap = null,
                ValueMap = "**{0}**",
                ValueSeparator = " • "
            }).Index;
            _receiverConfig.Name = config.Name;

            // group 2: Host Triggers
            _triggerNodeGroup = Display.AddGroup(new StringNodeGroup
            {
                TitleMap = "**{0}**:",
                Title = "Triggers",
                ContentMap = "• **User**: {0}",
                ValueMap = "`{0}`",
                ValueSeparator = " ",
                PageValueLimit = null
                //, StaticIndexer = true
            }).Index;

            _triggers = new List<LobbyTrigger<bool>>();

            // root commands for a user to join/leave
            _triggers.Add(new LobbyTrigger<bool>("join"));
            _triggers.Add(new LobbyTrigger<bool>("leave"));

            // commands that focus on the game
            _triggers.Add(new LobbyTrigger<bool>("start", true, isTask: true));

            // opens a local panel that only everyone from the host's receiver can see.
            _triggers.Add(new LobbyTrigger<bool>("config", true, true));
            
            // args based on users
            _triggers.Add(new LobbyTrigger<bool>("kick", true, args: new LobbyTriggerArg("user", typeof(string))));
            
            //_triggers.Add(new InvokerTrigger<bool>("ban", true, args: new InvokerArg("user", typeof(string))));

            // only usable if any local panel is open.
            _triggers.Add(new LobbyTrigger<bool>("close", true, true, isValid: false));

            // only usable when config panel is open.
            _triggers.Add(new LobbyTrigger<bool>("host", true, isValid: false, args: new LobbyTriggerArg("user", typeof(string))));
            _triggers.Add(new LobbyTrigger<bool>("title", true, true, isValid: false, args: new LobbyTriggerArg("title", typeof(string))));
            _triggers.Add(new LobbyTrigger<bool>("game", true, true, isValid: false, args: new LobbyTriggerArg("game_type", typeof(GameMode))));
            _triggers.Add(new LobbyTrigger<bool>("privacy", true, true, isValid: false, args: new LobbyTriggerArg("privacy_type", typeof(string))));

            _triggers.Add(new LobbyTrigger<bool>("closereceiver", true, isValid: false, args: new LobbyTriggerArg("receiver_id", typeof(ulong))));

            // opens another internal panel
            _triggers.Add(new LobbyTrigger<bool>("receivers", true, true, isValid: false));
            //_triggers.Add(new InvokerTrigger<bool>("bans", true, true, isValid: false));
            // ban
            _bans = new List<ulong>();
            _triggerLocalNodes = new List<int?>();

            _isConfigOpen = false;
            // set the initial host and guild.
            SetHostAsync(context).ConfigureAwait(false);
        }

        public string Name
        {
            get
            {
                StringNodeGroup group = Display.Groups.First(x => x.Index == _consoleNodeGroup);
                return !string.IsNullOrWhiteSpace(group.TitleMap) ? string.Format(group.TitleMap, group.Title) : (group.Title ?? $"#**{Id}**");
            }
        }

        public string Summary 
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"#**{Id}**: {Name}");
                sb.AppendLine($"{Mode.ToString()} **({Users.Count}/{Criteria.UserLimit})**");
                return sb.ToString();
            }
        }

        public string Counter
        {
            get
            {
                return $"({Users.Count}/{Criteria.UserLimit})";
            }
        }
        // that can be executed
        public bool IsRunning { get; private set; } // if the lobby is currently active or not.
        public bool IsGameStarted { get; private set; } // if the lobby is in-game.
        public bool Closed { get; private set; } // if the lobby shutdown. 
        public List<Receiver> Receivers { get; } // all linking channels
        public User Host { get { return Users.FirstOrDefault(x => x.IsHost); } }
        public List<User> Users { get; } // all connected users
        public Display Display { get; } // the display node used.
        private List<ulong> _bans { get; } // a list of user ids that cannot be read from.
        public bool FromReceiver(ulong channelId)
            => Receivers.Any(x => x.ChannelId.Value == channelId);

        private OriCommandContext _context;
        // begins the new lobby with a host and whatknot.
        public async Task StartAsync(OriCommandContext context)
        {
            _context = context;
            if (IsRunning)
            {
                Console.WriteLine("[Debug] -- The lobby has already started. Adding a user connection instead. --");
                await AddUserAsync(context);
                return;
            }

            IsRunning = true;

            // triggers are events that alter the lobby
            // examples include closing the lobby
            // starting the game, and so forth.
            //triggers.Add(new InvokerTrigger<bool>("sethost", true));

            async Task Handle(SocketMessage message)
            {

                ulong id = message.Author.Id;
                ulong receiverId = Receivers.First(x => x.ChannelId == message.Channel.Id).Id;
                Host.GuildIds[0] = receiverId;
                if (id == _rootClient.CurrentUser.Id)
                    return;
                if (!FromReceiver(message.Channel.Id))
                    return;

                if (!HasUser(id) && message.Content != "join")
                    return;

                //string msg = message.Content;
                // command parsing; allow arguments
                // make a new way to parse commands for a lobby

                string msg = message.ToString();
                string triggerKey = OriRegex.GetTriggerKey(msg);
                Console.WriteLine(triggerKey);
                if (_triggers.Any(x => x.Name == triggerKey))
                {
                    LobbyTrigger<bool> trigger = _triggers.First(x => x.Name == triggerKey);

                    if (trigger.RequireHost)
                    {
                        if (Host.Id != id)
                        {
                            await AddThenUpdateAsync($"You must be the host to use this command.", message.Author.Username);
                            goto DeleteMessage;
                        }
                    }

                    // prevents an invalid command from being used.
                    if (!trigger.IsValid)
                        goto DeleteMessage;

                    List<string> triggerArgs = OriRegex.GetTriggerArgs(triggerKey, msg);
                    if (triggerArgs == null) // an error occured if this happened
                        goto DeleteMessage;

                    switch (trigger.Name)
                    {
                        case "join":
                            if (HasUser(id))
                                break;
                            if (!_context.Container.TryGetUser(message.Author.Id, out OriUser user))
                                break;
                            await AddUserAsync(new User(user, receiverId));
                            break;
                        case "start":
                            if (Users.Count < Criteria.RequiredUsers)
                            {
                                await AddThenUpdateAsync($"Not enough users to start the game. ({Users.Count}/{Criteria.RequiredUsers})", Host.Name);
                                break;
                            }
                            if (trigger.IsTask)
                                _triggers.First(x => x.Name == trigger.Name).SetResult(true);
                            break;
                        case "leave":
                            await RemoveUserAsync(id);
                            break;
                        case "host":
                            if (triggerArgs.Count == trigger.Args.Count)
                            {
                                string userName = triggerArgs[0];
                                bool isValidId = ulong.TryParse(userName, out ulong userId);
                                User lobbyUser = HasUser(userName) ? Users.First(x => x.Name == userName) : isValidId ? HasUser(userId) ? Users.First(x => x.Id == userId) : Host : Host;

                                if (userName == Host.Name || userId == Host.Id)
                                {
                                    await AddThenUpdateAsync("You cannot kick yourself.", Host.Name);
                                    break;
                                }

                                if (Host.Id == lobbyUser.Id || Host.Name == lobbyUser.Name)
                                {
                                    await AddThenUpdateAsync("There is no matching user.", Host.Name);
                                    break;
                                }

                                Console.WriteLine($"Selected User: {lobbyUser.Name} _ {lobbyUser.Id}");
                                await SetHostAsync(lobbyUser.Id);
                            }
                            break;
                        case "title":
                            if (triggerArgs.Count > 0)
                            {
                                await SetNameAsync(string.Join(' ', triggerArgs));
                            }
                            break;
                        case "game":
                            if (triggerArgs.Count == trigger.Args.Count)
                            {
                                try
                                {
                                    if (EnumParser.TryParseEnum(triggerArgs[0], out GameMode type))
                                        await SetGameAsync(type);
                                    break;
                                }
                                catch (Exception)
                                { break; }
                            }
                            break;
                        case "privacy":
                            if (triggerArgs.Count == trigger.Args.Count)
                            {
                                if (triggerArgs[0] == "global")
                                    await SetPrivacyAsync(true);
                                else if (triggerArgs[0] == "local")
                                    await SetPrivacyAsync(false);

                                await AddThenUpdateAsync("Privacy set.");
                            }
                            break;
                        case "closereceiver":
                            if (triggerArgs.Count == trigger.Args.Count)
                            {
                                bool isValidId = ulong.TryParse(triggerArgs[0], out ulong internalReceiverId);
                                if (isValidId)
                                    if (Receivers.Any(x => x.ChannelId == internalReceiverId))
                                    {
                                        if (receiverId == internalReceiverId)
                                        {
                                            await AddThenUpdateAsync("You can't disconnect your own receiver.", Host.Name);
                                            break;
                                        }

                                        await RemoveReceiverAsync(internalReceiverId);
                                        await AddThenUpdateAsync("Receiver closed.");
                                        break;
                                    }
                            }
                            break;
                        case "kick":
                            if (triggerArgs.Count == trigger.Args.Count)
                            {
                                string userName = triggerArgs[0];
                                bool isValidId = ulong.TryParse(userName, out ulong userId);
                                User lobbyUser = HasUser(userName) ? Users.First(x => x.Name == userName) : isValidId ? HasUser(userId) ? Users.First(x => x.Id == userId) : Host : Host;

                                if (userName == Host.Name || userId == Host.Id)
                                {
                                    await AddThenUpdateAsync("You cannot kick yourself.", Host.Name);
                                    break;
                                }

                                if (Host.Id == lobbyUser.Id || Host.Name == lobbyUser.Name)
                                {
                                    await AddThenUpdateAsync("There is no matching user.", Host.Name);
                                    break;
                                }

                                Console.WriteLine($"Selected User: {lobbyUser.Name} _ {lobbyUser.Id}");
                                await RemoveUserAsync(lobbyUser.Id);
                            }
                            break;
                        case "receivers":
                            if (_localNodeGroup.HasValue)
                                Display.Remove(_localNodeGroup.Value);

                            _localNodeGroup = Display.AddGroup(new StringNodeGroup
                            {
                                ReceiverId = receiverId,
                                Title = "Receivers",
                                TitleMap = "**{0}**",
                                ContentMap = "All receivers are currently synchronized.\n{0}",
                                ValueMap = "• {0}",
                                AllowFormatting = true
                            }).Index;

                            foreach (Receiver receiver in Receivers)
                                Display.AddAtGroup(_localNodeGroup.Value, $"`{receiver.Id}`: `{receiver.SyncKey}`");

                            

                            if (_isConfigOpen)
                            {
                                if (_triggerReceiverNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerReceiverNode.Value);
                                _triggers.First(x => x.Name == "receivers").IsValid = false;

                                if (_triggerPrivacyNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerPrivacyNode.Value);
                                _triggers.First(x => x.Name == "privacy").IsValid = false;

                                if (_triggerGameNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerGameNode.Value);
                                _triggers.First(x => x.Name == "game").IsValid = false;

                                if (_triggerTitleNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerTitleNode.Value);
                                _triggers.First(x => x.Name == "title").IsValid = false;

                                if (_triggerHostNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerHostNode.Value);
                                _triggers.First(x => x.Name == "host").IsValid = false;

                                _isConfigOpen = false;
                            }

                            if (_isReceiversOpen)
                            {
                                if (_triggerCloseReceiverNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerCloseReceiverNode.Value);
                                _triggers.First(x => x.Name == "closereceiver").IsValid = false;
                                _isReceiversOpen = false;
                            }

                            if (_triggerCloseNode.HasValue)
                                Display.RemoveAtGroup(_triggerNodeGroup, _triggerCloseNode.Value);

                            //_triggerReceiverNode = Display.AddAtGroup(_triggerNodeGroup, "closereceiver", receiverId);
                            //_triggers.First(x => x.Name == "closereceiver").IsValid = true;

                            _triggerCloseNode = Display.AddAtGroup(_triggerNodeGroup, "close", receiverId).Index;
                            _triggers.First(x => x.Name == "close").IsValid = true;

                            _isReceiversOpen = true;

                            break;
                        case "config":
                            if (_localNodeGroup.HasValue)
                                Display.Remove(_localNodeGroup.Value);

                            _localNodeGroup = Display.AddGroup(new StringNodeGroup
                            {
                                ReceiverId = receiverId,
                                Title = "Lobby Config",
                                TitleMap = "**{0}**",
                                ContentMap = "{0}",
                                ValueMap = "{0}",
                                AllowFormatting = true
                            }).Index;

                            Display.AddAtGroup(_localNodeGroup.Value, $"**Host**: {Host.Name}");
                            Display.AddAtGroup(_localNodeGroup.Value, $"**Title**: {Name}");
                            Display.AddAtGroup(_localNodeGroup.Value, $"**Game**: {Mode}");
                            Display.AddAtGroup(_localNodeGroup.Value, $"**Privacy**: {(_isGlobal ? "Global" : "Local")}");
                            Display.AddAtGroup(_localNodeGroup.Value, $"**Receivers**: **{Receivers.Count}** Connected");

                            if (_isConfigOpen)
                            {
                                if (_triggerReceiverNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerReceiverNode.Value);
                                _triggers.First(x => x.Name == "receivers").IsValid = false;

                                if (_triggerPrivacyNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerPrivacyNode.Value);
                                _triggers.First(x => x.Name == "privacy").IsValid = false;

                                if (_triggerGameNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerGameNode.Value);
                                _triggers.First(x => x.Name == "game").IsValid = false;

                                if (_triggerTitleNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerTitleNode.Value);
                                _triggers.First(x => x.Name == "title").IsValid = false;

                                if (_triggerHostNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerHostNode.Value);
                                _triggers.First(x => x.Name == "host").IsValid = false;
                            }

                            if (_isReceiversOpen)
                            {
                                if (_triggerCloseReceiverNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerCloseReceiverNode.Value);
                                _triggers.First(x => x.Name == "closereceiver").IsValid = false;
                                _isReceiversOpen = false;
                            }

                            if (_triggerCloseNode.HasValue)
                                Display.RemoveAtGroup(_triggerNodeGroup, _triggerCloseNode.Value);

                            _triggerCloseNode = Display.AddAtGroup(_triggerNodeGroup, "close", receiverId).Index;
                            _triggers.First(x => x.Name == "close").IsValid = true;

                            _triggerHostNode = Display.AddAtGroup(_triggerNodeGroup, "host", receiverId).Index;
                            _triggers.First(x => x.Name == "host").IsValid = true;

                            _triggerTitleNode = Display.AddAtGroup(_triggerNodeGroup, "title", receiverId).Index;
                            _triggers.First(x => x.Name == "title").IsValid = true;

                            _triggerGameNode = Display.AddAtGroup(_triggerNodeGroup, "game", receiverId).Index;
                            _triggers.First(x => x.Name == "game").IsValid = true;

                            _triggerPrivacyNode = Display.AddAtGroup(_triggerNodeGroup, "privacy", receiverId).Index;
                            _triggers.First(x => x.Name == "privacy").IsValid = true;

                            _triggerReceiverNode = Display.AddAtGroup(_triggerNodeGroup, "receivers", receiverId).Index;
                            _triggers.First(x => x.Name == "receivers").IsValid = true;

                            _isConfigOpen = true;

                            break;
                        case "close":
                            if (_localNodeGroup.HasValue)
                            {
                                Display.Remove(_localNodeGroup.Value);
                                _localNodeGroup = null;
                            }
                            
                            if (_isConfigOpen)
                            {
                                if (_triggerReceiverNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerReceiverNode.Value);
                                _triggerReceiverNode = null;
                                _triggers.First(x => x.Name == "receivers").IsValid = false;

                                if (_triggerPrivacyNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerPrivacyNode.Value);
                                _triggerPrivacyNode = null;
                                _triggers.First(x => x.Name == "privacy").IsValid = false;

                                if (_triggerGameNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerGameNode.Value);
                                _triggerGameNode = null;
                                _triggers.First(x => x.Name == "game").IsValid = false;

                                if (_triggerTitleNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerTitleNode.Value);
                                _triggerTitleNode = null;
                                _triggers.First(x => x.Name == "title").IsValid = false;

                                if (_triggerHostNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerHostNode.Value);
                                _triggerHostNode = null;
                                _triggers.First(x => x.Name == "host").IsValid = false;

                                _isConfigOpen = false;
                            }

                            if (_isReceiversOpen)
                            {
                                if (_triggerCloseReceiverNode.HasValue)
                                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerCloseReceiverNode.Value);
                                _triggerCloseReceiverNode = null;
                                _triggers.First(x => x.Name == "closereceiver").IsValid = false;
                                _isReceiversOpen = false;
                            }

                            if (_triggerCloseNode.HasValue)
                                Display.RemoveAtGroup(_triggerNodeGroup, _triggerCloseNode.Value);
                            _triggerCloseNode = null;

                            _triggers.First(x => x.Name == "close").IsValid = false;


                            break;
                    }
                }
                else
                {
                    string _msg = msg.Replace('\n', ' ').Replace("`", "");
                    
                    Display.AddAtGroup(_consoleNodeGroup, $"[{message.Author.Username}] {(_msg.Length > 64 ? _msg.Substring(0, 64) : _msg)}");
                }

                DeleteMessage:
                    await SyncNodesAsync();
                    if (Receivers.First(x => x.ChannelId == message.Channel.Id).DeleteMessages)
                        await message.DeleteAsync();
                return;
            }
            _rootClient.MessageReceived += Handle;
            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (LobbyTrigger<bool> trigger in _triggers)
            {
                tasks.Add(trigger.Source.Task);
            }

            Task task = await Task.WhenAny(tasks).ConfigureAwait(false);
            _rootClient.MessageReceived -= Handle;

            if (task == _triggers.First(x => x.Name == "start").Source.Task)
            {
                await AddThenUpdateAsync($"{Mode.ToString()} is starting in 5 seconds.");
                
                //await Listen();
                /*

                - open internal game invoker
                - override display node
                - when the game invoker closes, return to the lobby display node.


                 */
                await Task.Delay(TimeSpan.FromSeconds(5));
                await CloseAsync();
            }
        }

        private async Task AddThenUpdateAsync(string content, string author = null)
        {
            Display.AddAtGroup(_consoleNodeGroup, $"[{(!string.IsNullOrWhiteSpace(author) ? $"To {author}" : "Console")}] {content}");
            await SyncNodesAsync();
        }

        private async Task SetNameAsync(string name)
        {
            Display.UpdateGroup(_consoleNodeGroup, name);
            await AddThenUpdateAsync($"{Host.Name} has updated the lobby name.");
        }

        private async Task SetGameAsync(GameMode gameType)
        {
            Mode = gameType;
            await AddThenUpdateAsync($"{Host.Name} has updated the game mode.");
        }
        private async Task SetPrivacyAsync(bool privacy)
        {
            _isGlobal = privacy;
        }
        private async Task SetHostAsync(OriCommandContext context)
        {
            // check if there is a host first.
            if (Host != null)
            {
                Console.WriteLine("[Debug] -- A host already exists. --");
                return;
            }
            else
            {
                User user = new User(context.Account, context.Guild.Id); // set host
                user.Tags = UserTag.Host;
                Users.Add(user);
                
                Display.AddAtGroup(_userNodeGroup, user.ToString());
                Display.UpdateGroup(_userNodeGroup, $"Users **{Counter}**");

                _localReceiverId = context.Guild.Id;

                foreach (LobbyTrigger<bool> trigger in _triggers)
                {
                    if (!trigger.IsValid)
                        continue;
                    Display.AddAtGroup(_triggerNodeGroup, trigger.Name, trigger.IsLocal ? (ulong?)_localReceiverId : null);
                }

                if (!HasNodeReceiver(context.Guild.Id))
                    await AddReceiverAsync(context.Guild);
            }
        }
        private async Task SetHostAsync(ulong id) //OriLobbyUser user
        {
            // check if the user asking to be set exists first:
            if (!HasUser(id))
            {
                Console.WriteLine("This user does not exist.");
                return;
            }
            // check if there is a host first
            if (Host != null)
            {
                User oldHost = Host;
                oldHost.Tags = UserTag.None;

                Display.UpdateAtGroup(_userNodeGroup, Users.IndexOf(oldHost), oldHost.ToString());
            }
            User user = Users.First(x => x.Id == id);
            Users.First(x => x.Id == id).Tags = UserTag.Host;
            Display.UpdateAtGroup(_userNodeGroup, Users.IndexOf(user), user.ToString());
            // gets rid of the local node displays, and sets the stuff to work within the other end now

            if (_localNodeGroup.HasValue)
            {
                Display.Remove(_localNodeGroup.Value);
                _localNodeGroup = null;
            }

            if (_isConfigOpen)
            {
                if (_triggerReceiverNode.HasValue)
                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerReceiverNode.Value);
                _triggers.First(x => x.Name == "receivers").IsValid = false;

                if (_triggerPrivacyNode.HasValue)
                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerPrivacyNode.Value);
                _triggers.First(x => x.Name == "privacy").IsValid = false;

                if (_triggerGameNode.HasValue)
                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerGameNode.Value);
                _triggers.First(x => x.Name == "game").IsValid = false;

                if (_triggerTitleNode.HasValue)
                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerTitleNode.Value);
                _triggers.First(x => x.Name == "title").IsValid = false;

                if (_triggerHostNode.HasValue)
                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerHostNode.Value);
                _triggers.First(x => x.Name == "host").IsValid = false;

                _isConfigOpen = false;
            }

            if (_isReceiversOpen)
            {
                if (_triggerCloseReceiverNode.HasValue)
                    Display.RemoveAtGroup(_triggerNodeGroup, _triggerCloseReceiverNode.Value);
                _triggerCloseReceiverNode = null;
                _triggers.First(x => x.Name == "closereceiver").IsValid = false;
                _isReceiversOpen = false;
            }

            if (_triggerCloseNode.HasValue)
                Display.RemoveAtGroup(_triggerNodeGroup, _triggerCloseNode.Value);

            await AddThenUpdateAsync($"{user.Name} is now the host.");
        }

        // adds a user using the context from the executed command
        public async Task AddUserAsync(OriCommandContext context)
        {
            if (!HasNodeReceiver(context.Guild.Id))
                await AddReceiverAsync(context.Guild);

            User user = new User(context.Account, context.Guild.Id);
            Users.Add(user);
            Display.AddAtGroup(_userNodeGroup, user.ToString());
            Display.UpdateGroup(_userNodeGroup, $"Users **{Counter}**");
            await AddThenUpdateAsync($"{context.User.Username} has joined.");
            Console.WriteLine(Counter);
        }

        private async Task AddUserAsync(User user)
        {
            Users.Add(user);
            Display.AddAtGroup(_userNodeGroup, user.ToString());
            Display.UpdateGroup(_userNodeGroup, $"Users **{Counter}**");
            await AddThenUpdateAsync($"{user.Name} has joined.");
            Console.WriteLine(Counter);
        }

        // adds a user using the context from the executed command
        private async Task RemoveUserAsync(ulong id)
        {
            if (HasUser(id))
            {
                User user = Users.First(x => x.Id == id);
                if (Users.Count - 1 > 0)
                {
                    if (Host.Id == user.Id)
                    {
                        User newHost = Users.Where(x => !x.IsHost).OrderBy(x => x.JoinedAt).First();
                        Host.Tags = UserTag.None;
                        Users.First(x => x == newHost).Tags = UserTag.Host;
                        Display.AddAtGroup(_consoleNodeGroup, $"{newHost.Name} is now the new host.");
                        Display.UpdateAtGroup(_userNodeGroup, Users.IndexOf(newHost), newHost.ToString());
                    }
                }
                else
                {
                    await CloseAsync();
                    return;
                }

                Display.RemoveAtGroup(_userNodeGroup, Users.IndexOf(user));
                Users.Remove(user);
                Display.UpdateGroup(_userNodeGroup, $"Users **{Counter}**");
                await AddThenUpdateAsync($"{user.Name} has left.");
                Console.WriteLine(Counter);
                return;
            }
        }

        private Receiver GetNodeReceiver(ulong guildId)
        {
            if (!Receivers.Any(x => x.Id == guildId))
                return null;
            else
                return Receivers.First(x => x.Id == guildId);
        }

        private bool HasUser(string name)
            => Users.Any(x => x.Name == name);

        public bool HasUser(ulong id)
            => Users.Any(x => x.Id == id);

        public User GetUser(ulong id)
            => HasUser(id) ? Users.First(x => x.Id == id) : null;

        public async Task CloseAsync()
        {
            Closed = true;
            foreach (Receiver receiver in Receivers)
                receiver.CloseAsync("The lobby is now closing.", TimeSpan.FromSeconds(3)).ConfigureAwait(false); // close all nodes

            Receivers.Clear(); // remove all nodes
            Display.Clear(); // reset the display.
            Users.Clear(); // remove all users
            IsRunning = false;
        }

        public bool HasNodeReceiver(ulong guildId)
            => Receivers.Any(x => x.Id == guildId);

        // connect a guild to the receivers
        public async Task AddReceiverAsync(SocketGuild guild)
        {
            if (HasNodeReceiver(guild.Id))
            {
                Console.WriteLine($"[Debug] -- A receiver already exists for {guild.Id}. --");
                return;
            }

            try
            {
                Receiver nodeReceiver = new Receiver(guild, _receiverConfig);
                Receivers.Add(nodeReceiver);
                Console.WriteLine($"[Debug] -- Receiver bound to {guild.Id}. --");
                await SyncNodesAsync();
            }
            catch (MissingGuildPermissionsException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task RemoveReceiverAsync(ulong guildId)
        {
            if (HasNodeReceiver(guildId))
            {
                await Receivers.First(x => x.Id == guildId).CloseAsync("Receiver closing.", TimeSpan.FromSeconds(3));
                Receivers.RemoveAll(x => x.Id == guildId);
                Users.Where(x => x.GuildIds[0] == guildId).ToList().ForEach(async x => await RemoveUserAsync(x.Id));
            }
        }

        public async Task SyncNodesAsync()
        {
            foreach (Receiver nodeReceiver in Receivers)
            {
                await nodeReceiver.UpdateAsync(_rootClient, Display);
                Console.WriteLine("[Debug] -- Receiver synchronized. --");
            }
        }
    }
}
