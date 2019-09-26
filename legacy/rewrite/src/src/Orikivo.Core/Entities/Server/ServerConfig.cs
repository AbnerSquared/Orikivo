using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class IconManager
    {
        // the collection of icons orikivo should use when displaying emojis.
    }

    public class ServerEvents
    {
        // keeps track of server events
    }

    public class UserEvents
    {
        // keeps track of personal events
    }

    public class Event
    {

    }

    public class ServerConfig
    {
        // This edits how orikivo should run
        public ServerConfig()
        {

        }

        public InsultType? InsultLevel { get; set; } // The insult level of orikivo. null by default, since it falls back to users.
        public bool QueueLooping {get; set;} // Allows the toggle of a looping queue.
        public bool LockedQueue { get; set; } // Allows the toggle of a locked queue, in which nothing can go in or out.
        public QueueSortingType QueueSorting { get; set;} = QueueSortingType.SongAdded;
        public ResultReturnType ResultType { get; set;} = ResultReturnType.First;
        public bool AutoResult {get; set;} = false; // Allows the toggle of auto sending the first result if no response was given.
        public string ModeratorPrefix {get; set;}
        public string ClosingModeratorPrefix {get; set;}
        public bool Public { get; set; } = true; // Toggles the inclusion of servers being listed in guildinfo

        public bool AutoKickInactiveUsers { get; set; } // toggles the kicking of inactive users that stay inactive for more than n amount of time.
        public TimeSpan InactivityLimit { get; set; } // limit of time a user can be inactive.

        public ulong InboundChannel { get; set; } // The channel of which [msg messages are sent. falls back to default channel
        // and then to the first usable channel.
        public List<ulong> AssignableRoles {get; set;} // The list of assignable roles that a user can add upon themselves.
        //public List<CompactMessage> Leavings { get; set; } // The message Orikivo sends upon leaving users.
        //public List<CompactMessage> Greetings { get; set; } // The message Orikivo sends upon new users. If there is more than one, it randomizes.

        public bool ForceAuthentication { get; set; } // If this bot went offline, and came back on.
                                           // if a user executes any command, and they lack the default role,
                                           // the verification will pop up again.
                                           // Determines if this server requires something to be accepted
                                           // before doing anything. (a CAPTCHA check)
                                           // Once verified, they will be given Config.DefaultRole.
        public bool CheckLinks { get; set; } // links sent will be verified.

        public List<string> Blacklist { get; set; } // a list of words you wish the bot to automatically censor upon matching.
        // Perhaps make a greeting system that allows a roulette of messages.
        public List<ulong> ModeratorRoles { get; set; } // The list of roles that can use Orikivo's moderator commands.
        public ulong DefaultRole { get; set; } // The default role for a user upon joining a server.
        public string Prefix { get; set; } = "["; // The prefix for the server.
        public string ClosingPrefix { get; set; } = "]"; // The closing prefix for the server.
        public List<ulong> BlockedUsers { get; set; } // The list of people blocked from using orikivo.
        public ulong DefaultChannel { get; set; } // the default channel for all untracked systems.
        public ulong DefaultVoiceChannel { get; set; } // The default voice channel for all audio systems.
        public bool Throw { get; set; } // Toggles command exceptions upon one occuring
        public bool DeveloperLock { get; set; } = false;// Locks all config to only be edited by the bot dev.
        //public List<GlobalModule> DisabledModules {get; set;} // A list of modules able to opted out of.
        public bool Notifications { get; set; } // Allows or disallows notifications. Overrides subnotifications
        public ServerNotifiers Notifiers { get; set; } // Holds all notifications possible. If Notifications is inactive, ignore.
        public bool UsePrefixes { get; set; } = true; // Allows the server to allow or block the usage of prefixes.
        public bool Safeguard {get; set;} // Prevents nsfw command logic, and explicit content. Overrides NSFW
        public bool SafeChat { get; set; } // Prevents swearing from passing through global communication.
        public bool CrossChat { get; set; } // toggles cross-server communication.
        public SafetyType NsfwLogic { get; set; } // How nsfw commands are shown. disabled, on marked areas, or globally.
        public bool TrackLocal { get; set; } = true; // tracks local experience earned.
        public bool TrackActive { get; set; } = true; // tracks activity experience.
        public bool OverrideAccountConfig { get; set; } = true; // determines if it overrides account configuration.
        public IconManager Emojis { get; set; } // the collection of emoticons used for orikivo.
        public bool ThrowUnknown { get; set; } = false;

        public void ToggleExceptions()
        {
            Throw = !Throw;
        }

        public void SetInboundChannel(SocketChannel c)
            => SetInboundChannel(c.Id);

        public void ToggleSafeChat()
            => SafeChat = !SafeChat;

        public void SetInboundChannel(ulong id)
        {
            InboundChannel = id;
        }

        public void ToggleCrossChat()
        {
            CrossChat = !CrossChat;
        }

        public void TogglePublic()
        {
            Public = !Public;
        }

        public string GetPrefix(OrikivoCommandContext ctx)
        {
            if (!UsePrefixes)
            {
                return ctx.Client.CurrentUser.Mention + " ";
            }
            return Prefix;
        }


        public void UpdatePrefix(string s)
        {
            Prefix = s;
        }

    }
}