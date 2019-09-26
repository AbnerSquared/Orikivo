using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    class Server2
    {
        public ulong Id { get; set; }
        public ulong Creator { get; set; }
        public ulong Name { get; set; }
        public QueueCollection Queue { get; set; }
        public ActivityTracker ActiveData { get; set; }
        public ServerCooldownManager Cooldowns {get; set;}
        public ServerOptions Config { get; set; }
        public EventCollection Events { get; set; }
        public CustomCollection Customs { get; set; }

    }

    class ServerOptions
    {
        public ServerOptions()
        {
            DevLock = false;
            Exceptions = true;
            IgnoreAccountConfig = false;
            CrossChat = false;
            UsePrefixes = true;
            WordGuard = false;
            LinkGuard = false;
            LoopQueue = false;
            LockQueue = false;
            AutoReturn = false;
            AutoKick = false;
            UseResponses = null;
            UseGreetings = UseResponses ?? false;
            UseLeavings = UseResponses ?? false;
            Visibility = Visibility.Public;
            SafeGuard = SafetyType.Enabled;
            QueueSorting = QueueSequence.Song;
            Locale = Locale.English;
            SledgePower = SledgePower.Polite;
            AdminRole = null;
            ModRole = null;
            HelperRole = null;
            MusicRole = null;
            DefaultRole = null;
            BotRole = null;
            DefaultChannel = null;
            DefaultVoiceChannel = null;
            RelayPoint = null;
            Prefix = "[";
            ClosingPrefix = "]";
            ModPrefix = "__";
            Owners = new List<ulong>();
            AssignableRoles = new List<ulong>();
            WhitelistedServers = new List<ulong>();
            WordBlacklist = Global.GlobalWordBlacklist;
            SiteBlacklist = new List<string>();
            Greetings = new List<ResponseContext>();
            Leavings = new List<ResponseContext>();
            InactivitySpan = null;
            Icons = null;
            Modules = null;
        }

        [JsonIgnore]
        public static Range PrefixLimit = new Range(1, 16);

        [JsonIgnore]
        public static ServerOptions Default = new ServerOptions();

        public bool HasOpenGameSession
        {
            get
            {
                return GameSessions.Funct();
            }
        }

        public List<GameSession> GameSessions = new List<GameSession>();
        public bool DevLock { get; set; }
        public bool Exceptions { get; set; }
        public bool IgnoreAccountConfig { get; set; }
        public bool CrossChat { get; set; }
        public bool UsePrefixes { get; set; }
        public bool WordGuard { get; set; }
        public bool LinkGuard { get; set; }
        public bool LoopQueue { get; set; }
        public bool LockQueue { get; set; }
        public bool AutoReturn { get; set; }
        public bool AutoKick { get; set; }
        public bool? UseResponses { get; set; } // if the server is going to greet anything
        public bool UseGreetings { get; set; }
        public bool UseLeavings { get; set; }

        public Visibility Visibility { get; set; }
        public SafetyType SafeGuard { get; set; }
        public QueueSequence QueueSorting { get; set; }
        public Locale Locale { get; set; }
        public SledgePower SledgePower { get; set; }

        public ulong? AdminRole { get; set; }
        public ulong? ModRole { get; set; }
        public ulong? HelperRole { get; set; }
        public ulong? MusicRole { get; set; }
        public ulong? DefaultRole { get; set; }
        public ulong? BotRole { get; set; }
        public ulong? DefaultChannel { get; set; }
        public ulong? DefaultVoiceChannel { get; set; }
        public ulong? RelayPoint { get; set; }
        public ulong? ActionLog { get; set; } // where server actions can be logged.
        public int GameSessionLimit { get; set; }
        public bool ExternGameSessions { get; set; } // if game sessions are used externally, or in chat.
        /*
            Game Session Types:
            External: A seperate channel is built for the game session. A general category can be specified, or a permanent channel.
            Inline: The game is held inside a public channel, with actions being sent to DMs, and the display is constantly sent as a new msg.
             */
        // public List<ulong> GameSessionChannel {get; set;}
        // public ulong GameSessionCategory { get; set;}  // used for placing new game sessions into...

        private string _prefix;
        private string _closingPrefix;
        private string _modPrefix;

        public string Prefix { get { return UsePrefixes ? _prefix ?? Default.Prefix : $"{Global.Client.CurrentUser.Mention} "; } set { _prefix = value; } }
        public string ClosingPrefix { get { return _closingPrefix ?? Default.ClosingPrefix; } set { _closingPrefix = value; } }
        public string ModPrefix { get { return _modPrefix ?? Default.ModPrefix; } set { _modPrefix = value; } }

        public List<ulong> Owners { get; set; }
        public List<ulong> AssignableRoles { get; set; }
        public List<ulong> WhitelistedServers { get; set; }
        public List<string> WordBlacklist { get; set; }
        public List<string> SiteBlacklist { get; set; }

        public List<ulong> UserBlacklist { get; set; }
        public List<ulong> RoleBlacklist { get; set; }
        public List<ulong> ChannelBlacklist { get; set; }

        public List<ResponseContext> Greetings { get; set; }
        public List<ResponseContext> Leavings { get; set; }
        
        public TimeSpan? InactivitySpan { get; set; }
        public IconManager Icons { get; set; }
        public ModuleManagement Modules { get; set; }

        // these are shortcuts for things like: server count, servername, entry user name, join position, join date.
        private const string UserResponseContext = "/[u]/";
        /*
         /[@u]/ = User Mention.
         /[u]/ = User Name
         /[0u]/ = User Join Position
         /[s]/ = Server Name
         /[d]/ = User Join Date.
             
             */
        
        
        // welcome, \[u] to \[s]!
        private const string UserPositionResponseContext = "\\[up]"; // You are the \[up] member to join!
        private const string JoinDateResponseContext = "\\[d]";
        private const string ServerResponseContext = "\\[s]";
        private const string ServerCounterResponseContext = "\\[sc]";
        


        //public static string TryParseResponse(string s)
        //{

        //}

        /* Information */
        public string Read()
        {
            // Configuration
            StringBuilder s = new StringBuilder();

            //page1
            s.AppendLine($"Developer Lock".MarkdownBold());
            s.AppendLine($"*(Icon)* If a server requires that the bot be managed by the developer. Otherwise, this is disabled.");
            s.AppendLine($"Exceptions".MarkdownBold());
            s.AppendLine($"*(Icon)* If active, errors will be sent into the channel.");
            s.AppendLine($"Override Personal Config".MarkdownBold());
            s.AppendLine($"*(Icon)* If enabled, certain server options will outweigh main account features. An example is an account having **SafeGuard** enabled, while the server has it disabled.");
            s.AppendLine($"CrossChat".MarkdownBold());
            s.AppendLine($"*(Icon)* This gives the server the ability to speak with any server on the shard.");
            s.AppendLine($"Use Prefixes".MarkdownBold());
            s.AppendLine($"*(Icon)* This toggles the usage of prefixes on a server.");
            s.AppendLine($"WordGuard".MarkdownBold());
            s.AppendLine($"*(Icon)* If active, all messages sent will be checked for harsh language by default. Words may be added.");
            s.AppendLine($"LinkGuard".MarkdownBold());
            s.AppendLine($"*(Icon)* This watches over links, and automatically deletes a link that fits the link blacklist.");
            s.AppendLine($"Queue Looping".MarkdownBold());
            s.AppendLine($"*(Icon)* This toggles the queue looping when playing music.");
            s.AppendLine($"Queue Lock".MarkdownBold());
            s.AppendLine($"*(Icon)* This will prevent the queue from dropping songs, as well as entering songs.");
            s.AppendLine($"AutoReturn".MarkdownBold());
            s.AppendLine($"*(Icon)* (once **DynamicChat** is built) This will toggle the search function automatically returning the first result of a search if left unused.");


            //page2
            s.AppendLine($"AutoKick".MarkdownBold());
            s.AppendLine($"*(Icon)* This toggles automatically kicking members that remain inactive for a set amount of time.");
            s.AppendLine($"Visibility".MarkdownBold());
            s.AppendLine($"*(Icon)* This toggles the ability of other servers being able to view you.");

            return s.ToString();
        }

        /* Auto-Handler */
        public void Setup() {}
        public void ResetAll() {}

        public void ToggleDevLock() => DevLock = !DevLock;
        public void ToggleAccountConfig() => IgnoreAccountConfig = !IgnoreAccountConfig;
        public void ToggleExceptions() => Exceptions = !Exceptions;
        public void ToggleCrossChat() => CrossChat = !CrossChat;
        public void TogglePrefixes() => UsePrefixes = !UsePrefixes;
        public void ToggleWordGuard() => WordGuard = !WordGuard;
        public void ToggleLinkGuard() => LinkGuard = !LinkGuard;
        public void ToggleQueueLooping() => LoopQueue = !LoopQueue;
        public void ToggleQueueLock() => LockQueue = !LockQueue;
        public void ToggleAutoReturn() => AutoReturn = !AutoReturn;
        public void ToggleAutoKick() => AutoKick = !AutoKick;
        public void ToggleResponses()
        {
            if (UseResponses.HasValue)
                DisableResponses();
            else
                EnableResponses();
        }
        public void EnableResponses() => UseResponses = true;
        public void DisableResponses() => UseResponses = null;
        public bool TrySetAdminRole(SocketGuild g, ulong u)
        {
            if (!g.HasRole(u))
                return false;

            SetAdminRole(u);
            return true;
        }
        public bool HasAdminRole { get { return AdminRole.HasValue; } }
        private void SetAdminRole(ulong u)
            => AdminRole = u;
        public void ClearAdminRole()
            => AdminRole = null;
        public bool TrySetModRole(SocketGuild g, ulong u)
        {
            if (!g.HasRole(u))
                return false;

            SetModRole(u);
            return true;
        }
        public bool HasModRole { get { return ModRole.HasValue; } }
        private void SetModRole(ulong u)
            => ModRole = u;
        public void ClearModRole()
            => ModRole = null;
        public bool TrySetHelperRole(SocketGuild g, ulong u)
        {
            if (!g.HasRole(u))
                return false;

            SetHelperRole(u);
            return true;
        }
        public bool HasHelperRole { get { return HelperRole.HasValue; } }
        private void SetHelperRole(ulong u)
            => HelperRole = u;
        public void ClearHelperRole()
            => HelperRole = null;
        public bool TrySetMusicRole(SocketGuild g, ulong u)
        {
            if (!g.HasRole(u))
                return false;

            SetMusicRole(u);
            return true;
        }
        public bool HasMusicRole { get { return MusicRole.HasValue; } }
        private void SetMusicRole(ulong u)
            => MusicRole = u;
        public void ClearMusicRole()
            => MusicRole = null;
        public bool TrySetDefaultRole (SocketGuild g, ulong u)
        {
            if (!g.HasRole(u))
                return false;

            SetDefaultRole(u);
            return true;
        }
        public bool HasDefaultRole { get { return DefaultRole.HasValue; } }
        private void SetDefaultRole(ulong u)
            => DefaultRole = u;
        public void ClearDefaultRole()
            => DefaultRole = null;
        public bool TrySetBotRole(SocketGuild g, ulong u)
        {
            if (!g.HasRole(u))
                return false;

            SetBotRole(u);
            return true;
        }
        public bool HasBotRole { get { return BotRole.HasValue; } }
        private void SetBotRole(ulong u)
            => BotRole = u;
        public void ClearBotRole()
            => BotRole = null;
        public bool TrySetDefaultChannel(SocketGuild g, ulong u)
        {
            if (!g.HasRole(u))
                return false;

            SetDefaultChannel(u);
            return true;
        }
        public bool HasDefaultChannel { get { return DefaultChannel.HasValue; } }
        private void SetDefaultChannel(ulong u)
            => DefaultChannel = u;
        public void ClearDefaultChannel()
            => DefaultChannel = null;
        public bool TrySetDefaultVoice(SocketGuild g, ulong u)
        {
            if (!g.HasRole(u))
                return false;

            SetDefaultVoice(u);
            return true;
        }
        public bool HasDefaultVoice { get { return DefaultVoiceChannel.HasValue; } }
        private void SetDefaultVoice(ulong u)
            => DefaultVoiceChannel = u;
        public void ClearDefaultVoice()
            => DefaultVoiceChannel = null;
        public bool TrySetRelayPoint(SocketGuild g, ulong u)
        {
            if (!g.HasTextChannel(u))
                return false;

            SetRelayPoint(u);
            return true;
        }
        public bool HasRelayPoint { get { return RelayPoint.HasValue; } }
        private void SetRelayPoint(ulong u)
            => RelayPoint = u;
        public void ClearRelayPoint()
            => RelayPoint = null;
        public bool HasCustomClosingPrefix { get { return _closingPrefix.Exists() ? _closingPrefix == Default.ClosingPrefix : false; } }
        public bool HasCustomPrefix { get { return _prefix.Exists() ? _prefix == Default.Prefix : false; } }
        public bool HasCustomModPrefix { get { return _modPrefix.Exists() ? _modPrefix == Default.ModPrefix : false; } }
        public bool IsProperPrefix(string s)
            => s.Length.IsInRange(PrefixLimit);
        public bool IsAlreadyAnyPrefix(string s)
        {
            if (IsAlreadyPrefix(s))
                return true;
            if (IsAlreadyModPrefix(s))
                return true;
            if (IsAlreadyClosingPrefix(s))
                return true;
            return false;
        }
        public bool IsAlreadyClosingPrefix(string s)
            => _closingPrefix == s;
        public bool IsAlreadyModPrefix(string s)
            => _modPrefix == s;
        public bool IsAlreadyPrefix(string s)
            => _prefix == s;
        private void SetModPrefix(string s)
            => _modPrefix = s;
        private void SetPrefix(string s)
            => _prefix = s;
        private void SetClosingPrefix(string s)
            => _closingPrefix = s;
        public bool TrySetModPrefix(string s)
        {
            if (!IsProperPrefix(s))
                return false;
            if (HasCustomModPrefix)
            {
                if (IsAlreadyAnyPrefix(s))
                    return false;
            }

            SetPrefix(s);
            return true;
        }
        public bool TrySetPrefix(string s)
        {
            if (!IsProperPrefix(s))
                return false;
            if (HasCustomPrefix)
            {
                if (IsAlreadyAnyPrefix(s))
                    return false;
            }

            SetPrefix(s);
            return true;
        }
        public bool TrySetClosingPrefix(string s)
        {
            if (!IsProperPrefix(s))
                return false;
            if (HasCustomClosingPrefix)
            {
                if (IsAlreadyAnyPrefix(s))
                    return false;
            }

            SetPrefix(s);
            return true;
        }
        public void SetVisibility(Visibility v)
            => Visibility = v;
        public void ResetVisibility(Visibility v)
            => Visibility = Default.Visibility;
        public void SetQueueSort(QueueSequence q)
            => QueueSorting = q;
        public void ResetQueueSort()
            => QueueSorting = Default.QueueSorting;
        public void SetLocale(Locale l)
            => Locale = l;
        public void ResetLocale()
            => Locale = Default.Locale;
        public void SetSledgePower(SledgePower s)
            => SledgePower = s;
        public void ResetSledgePower()
            => SledgePower = Default.SledgePower;

        public void AddOwner() { }
        public void RemoveOwner() { }
        public void AddSar() { }
        public void RemoveSar() { }
        public void ClearSar() { }
        public void AddWhitelistedServer() { }
        public void RemoveWhitelistedServer() { }
        public void ClearWhitelistedServers() { }
        public void BlacklistWord() { }
        public void ResetWordBlacklist() { }
        public void BlacklistUrl() { }
        public void ResetUrlBlacklist() { }
        public void AddGreeting() { }
        public void RemoveGreeting() { }
        public void ClearGreetings() { }
        public void AddExit() { }
        public void RemoveExit() { }
        public void ClearExit() { }
        public void BlockModule() { }
        public void BlockCommand() { }
        public void UnblockModule() { }
        public void UnblockCommand() { }
        public void ResetModules() { }
        public void ResetCommands() { }
        public void SetInactivitySpan() { }
        public void ResetInactivitySpan() { }
        public void SetIcon() { }
        public void ResetIcon() { }
    }

    public class Notificator
    {
        public bool Mail { get; set; }
        public bool Levels { get; set; }
        public bool Status { get; set; }
        public bool Streams { get; set; }
        public bool Updates { get; set; }
    }

    class CustomCollection
    {

    }
    class ServerCooldownManager
    {

    }
    class ActivityTracker
    {

    }
    class ModuleManagement
    {

    }
    enum QueueSequence
    {
        Song = 1,
        User = 2
    }

    /// <summary>
    /// Represents a returning message context.
    /// </summary>
    class ResponseContext
    {

    }
    class QueueCollection
    {

    }
   


}
