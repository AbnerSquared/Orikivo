using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;

namespace Orikivo.Modules
{
    [Name("Options")]
    [Summary("Provides helpers for configurable settings.")]
    [DontAutoLoad]
    public class OptionsModule : ModuleBase<OrikivoCommandContext>
    {
        public OptionsModule() { }

        // get info on all possible values of an option.
        [Command("optionvalues")]
        public async Task OptionValueTypesResponseAsync()
        {

        }

        [Command("option")]
        [Summary("Learn about a specified option in depth.")]
        public async Task OptionResponseAsync
        (
            [Name("context"), Summary("The option to view context for.")] AccountOption option
        )
        {
            await ModuleManager.TryExecute(Context.Channel, OptionsService.ReadOptionAsync(Context, option));
        }

        // sort of like an advanced version of editing commands.
        [Command("setoption")]
        [Summary("Edit the value of a specified account option. This is dynamic, and will work off of what you provide.")]
        public async Task SetOptionAsync
        (
            [Name("context"), Summary("The option to view context for.")] AccountOption option,
            [Name("value"), Summary("The new value to be set in place of the option.")] object value = null
        )
        {
            await ModuleManager.TryExecute(Context.Channel, OptionsService.SetOptionAsync(Context, option, value));
        }

        [Command("alloptions")]
        [Summary("See all current options that derive from your account.")]
        public async Task OptionsResponseAsync()
        {
            await ModuleManager.TryExecute(Context.Channel, OptionsService.ReadOptionsAsync(Context));
        }

        [Name("UserOptions")]
        [Summary("Detailed controls for specific options.")]
        [DontAutoLoad]
        public class UserOptionsModule : ModuleBase<OrikivoCommandContext>
        {
            public UserOptionsModule() { }


            [Command("autofix")]
            public async Task AutoFixResponseAsync()
            {

            }

            [Command("autofixtoggle")]
            public async Task AutoFixToggleAsync()
            {

            }

            [Command("overflow")]
            public async Task OverflowResponseAsync()
            {

            }

            [Command("overflowtoggle")]
            public async Task OverflowToggleAsync()
            {

            }

            [Command("tooltips")]
            public async Task TooltipsResponseAsync()
            {

            }

            [Command("tooltipstoggle")]
            public async Task TooltipsToggleAsync()
            {

            }

            //nickname
            [Command("clearnickname")]
            public async Task ClearNicknameAsync()
            {

            }

            [Command("nickname"), Priority(0)]
            public async Task NicknameResponseAsync()
            {

            }

            [Command("nickname"), Priority(1)]
            public async Task NicknameResponseAsync(string name)
            {

            }

            [Command("clearprefix")]
            public async Task ClearPrefixAsync()
            {

            }

            [Command("prefix")]
            public async Task PrefixResponseAsync()
            {
                // make it to where if the user doesn't have a personal one set, it derives from the server-side, or the global default.
            }

            [Command("prefix")]
            public async Task SetPrefixAsync(string value)
            {

            }

            //end prefix
            [Command("clearendprefix")]
            public async Task ClearEndPrefixAsync()
            {

            }

            [Command("endprefix")]
            public async Task EndPrefixResponseAsync()
            {

            }

            [Command("endprefix")]
            public async Task SetEndPrefixAsync(string value)
            {

            }

            [Command("clearlocaleblacklist")]
            public async Task ClearLocaleBlacklistAsync()
            {

            }

            [Command("localeblacklist"), Alias("bannedwords")]
            public async Task LocaleBlacklistResponseAsync()
            {
                // see all banned words.
            }

            [Command("blockword")]
            public async Task BlockWordAsync(string word)
            {
                // add to the chat blacklist.
            }

            [Command("revokeword")]
            public async Task RevokeWord(string word)
            {

            }

            [Command("clearsiteblacklist")]
            public async Task ClearSiteBlacklistAsync()
            {

            }

            [Command("siteblacklist")]
            public async Task SiteBlacklistResponseAsync()
            {

            }

            [Command("blocksite")]
            public async Task BlockSiteAsync(string url)
            {

            }

            [Command("revokesite")]
            public async Task RevokeSiteAsync(string url)
            {

            }

            // this will remove all sites that start with this.
            [Command("revokebasesite")]
            public async Task RevokeMatchingBaseSitesAsync(string url)
            {

            }

            [Command("linking")]
            public async Task LinkingResponseAsync()
            {

            }

            [Command("linkingtoggle")]
            public async Task LinkingToggleAsync()
            {

            }

            [Command("exceptions"), Alias("throw")]
            public async Task ThrowResponseAsync()
            {

            }

            [Command("exceptionstoggle"), Alias("throwtoggle")]
            public async Task ThrowToggleAsync()
            {

            }

            [Command("safeguard")]
            public async Task SafeGuardResponseAsync()
            {

            }

            [Command("safeguardtoggle")]
            public async Task SafeGuardToggleAsync()
            {

            }

            [Command("outputformat")]
            public async Task OutputFormatResponseAsync()
            {

            }

            [Command("outputformat"), Priority(1)]
            public async Task SetOutputFormatAsync(OutputFormat format) // make an OutputFormat TypeReader
            {

            }

            [Command("nullformat")]
            public async Task NullFormatResponseAsync()
            {

            }

            [Command("nullformat"), Priority(1)]
            public async Task SetNullFormatAsync(NullObjectHandling value) // make a NullObjectHandling TypeReader.
            {

            }

            [Command("visibility")]
            public async Task VisibilityResponseAsync()
            {

            }

            [Command("visibility"), Priority(1)]
            public async Task SetVisibilityAsync(Visibility value) // make a Visibility TypeReader.
            {

            }

            [Command("clearsledge"), Alias("clearinsultpower")]
            public async Task ClearSledgeAsync()
            {

            }

            [Command("sledge"), Alias("insultpower")]
            public async Task SledgeResponseAsync()
            {

            }

            [Command("sledge"), Alias("insultpower"), Priority(1)]
            public async Task SetSledgeAsync(SledgePower power) // make a SledgePower TypeReader.
            {

            }

            [Command("iconformat")]
            public async Task IconFormatResponseAsync()
            {

            }

            [Command("iconformat"), Priority(1)]
            public async Task SetIconFormatAsync()
            {

            }

            [Command("portablemode")]
            public async Task TogglePortableModeAsync()
            {

            }

            [Command("clearlocale")]
            public async Task ClearLocaleAsync()
            {

            }

            [Command("locale")]
            public async Task LocaleResponseAsync()
            {

            }

            [Command("locale"), Priority(1)]
            public async Task SetLocaleAsync(Locale language) // locale typereader
            {

            }

            [Command("predecode")]
            public async Task PreDecodeResponseAsync()
            {

            }

            [Command("predecodetoggle")]
            public async Task PreDecodeToggleAsync()
            {

            }

            [Command("globaldecode")]
            public async Task GlobalDecodeResponseAsync()
            {

            }

            [Command("globaldecodetoggle")]
            public async Task GlobalDecodeToggleAsync()
            {

            }

            [Command("directional")]
            public async Task DirectionalResponseAsync()
            {

            }

            [Command("directionaltoggle")]
            public async Task DirectionalToggleAsync()
            {

            }

            [Command("usernameformat")]
            public async Task NameFormatResponseAsync()
            {

            }

            [Command("usernameformat"), Priority(1)]
            public async Task SetNameFormatAsync(NameFormat format) // namedisplayformat typereader...?
            {

            }

            [Command("matchhandling")]
            public async Task MatchHandlingResponseAsync()
            {

            }

            [Command("matchhandling"), Priority(1)]
            public async Task SetMatchHandlingAsync()
            {

            }

            [Command("searchhandling")]
            public async Task SearchHandlingResponseAsync()
            {

            }

            [Command("searchhandling"), Priority(1)]
            public async Task SetSearchHandlingAsync()
            {

            }

            [Command("wordguardtoggle")]
            public async Task WordGuardToggleAsync()
            {

            }
            [Command("wordguard")]
            public async Task WordGuardResponseAsync()
            {

            }

            [Command("wordguard"), Priority(1)]
            public async Task SetWordGuardAsync()
            {

            }

            [Command("colorformat")]
            public async Task ColorFormatResponseAsync()
            {

            }

            [Command("colorformat"), Priority(1)]
            public async Task SetColorFormatAsync()
            {

            }

            [Command("iconnameformat")]
            public async Task IconNameFormatResponseAsync()
            {

            }

            [Command("iconnameformattoggle")]
            public async Task IconNameFormatToggleAsync()
            {

            }
        }

        [Name("GuildOptions")]
        [Summary("Detailed controls for guild-specific options.")]
        [DontAutoLoad]
        public class GuildOptionsModule : ModuleBase<OrikivoCommandContext>
        {
            public GuildOptionsModule() { }

        }

        [Name("ClientModeration")]
        [Summary("Configurable client-side mechanics for Orikivo.")]
        [DontAutoLoad]
        public class ClientModerationModule : ModuleBase<OrikivoCommandContext>
        {
            [Command("mute")]
            public async Task MuteAsync() { }

            [Command("setmmrole")]
            public async Task SetMusicManagerRoleAsync() { }

            [Command("promotemm")]
            public async Task AddMusicManagerAsync() { }

            [Command("toggleentrymessages")]
            public async Task ToggleEntryMessagesAsync() { }

            [Command("togglegreetings")]
            public async Task ToggleGreetingsAsync() { }

            public async Task ToggleLeavingsAsync() { }

            public async Task DefaultEntryMessagesAsync() { }

            [Command("addgreeting")]
            public async Task AddGreetingAsync() { }

            [Command("removegreeting")]
            public async Task RemoveGreetingAsync() { }

            [Command("cleargreetings")]
            public async Task ClearGreetingsAsync() { }



            [Command("addleaving")]
            public async Task AddLeavingAsync() { }

            [Command("removeleaving")]
            public async Task RemoveLeavingAsync() { }

            [Command("clearleavings")]
            public async Task ClearLeavingsAsync() { }
        }

        [Name("DiscordModeration")]
        [Summary("Configurable client-side mechanics for servers.")]
        [DontAutoLoad]
        public class ModerationModule : ModuleBase<OrikivoCommandContext>
        {
            public ModerationModule() { }

            [Command("ban")]
            public async Task BanAsync() { }

            [Command("softban")]
            public async Task SoftBanAsync() { }

            [Command("hackban")]
            public async Task HackBanAsync() { }

            [Command("editguild")]
            public async Task EditGuildAsync() { }

            // give user role
            // give user nickname
            // take user role
            // reset nickname
            // whitelist user on a channel
            // etc...?

            // literally clear everything pertaining to them.
            [Command("kick")]
            public async Task KickUserAsync() { }

            [Command("createrole")]
            public async Task CreateRoleAsync() { }

            [Command("editrole")]
            public async Task EditRoleAsync() { }

            [Command("deleterole")]
            public async Task DeleteRoleAsync() { }

            [Command("createchannel")]
            public async Task CreateChannelAsync() { }

            [Command("editchannel")]
            public async Task EditChannelAsync() { }

            [Command("deletechannel")]
            public async Task DeleteChannelAsync() { }

            [Command("purge")]
            public async Task PurgeAsync() { }

            [Command("clonechannel")]
            public async Task CloneChannelAsync() { }

            [Command("clonerole")]
            public async Task CloneRoleAsync() { }

            [Command("createcategory")]
            public async Task CreateCategoryAsync() { }

            [Command("editcategory")]
            public async Task EditCategoryAsync() { }

            [Command("deletecategory")]
            public async Task DeleteCategoryAsync() { }

            [Command("clonecategory")]
            public async Task CloneCategoryAsync() { }

            // this user will no longer be able to use orikivo server-wide.
            [Command("block")]
            public async Task BlockUserAsync() { }

            // bans a user, and will automatically ban them in the case of which you're both the owner AND Orikivo is in the server.
            [Command("globalban")]
            public async Task GlobalBlockUserAsync() { }

            [Command("addemoji")]
            public async Task AddEmojiAsync() { }

            [Command("editemoji")]
            public async Task EditEmojiAsync() { }

            [Command("deleteemoji")]
            public async Task DeleteEmojiAsync() { }

            [Command("pin")]
            public async Task PinMessageAsync() { }

            [Command("unpin")]
            public async Task UnpinMessageAsync() { }

            // clones the current channel, and deletes the original one.
            [Command("clearchannel")]
            public async Task ClearChannelAsync() { }


            // spamguard
            // spamguard warnings
            // spamguard duration

            // mute
            // unmute

            // warn
            // unwarn
        }

        [Name("DevExec")]
        [Summary("Configuration mechanics only available to the bot developer(s).")]
        [DontAutoLoad]
        public class DevExecModule : ModuleBase<OrikivoCommandContext>
        {
            public DevExecModule() { }

            // completely block a user from using orikivo
            [Command("__prohibit")]
            public async Task DevBlockUserAsync() { }

            [Command("__revoke")]
            public async Task DevUnblockUserAsync() { }

            [Command("__unload")]
            public async Task UnloadModuleAsync() { }

            [Command("__disable")]
            public async Task DisableModuleAsync() { }

            // only the program reboots themselves
            // save a (if reboot) toggle, and have it to where it notifiies the last channel it was executed in to the user.
            [Command("__reboot")]
            public async Task RebootAsync() { }

            // restart the entire computer, and return to it having the host.
            [Command("__reboothost")]
            public async Task HostRebootAsync() { }

            // get a user's .json data.
            [Command("__getuser")]
            public async Task GetUserAsync() { }

            // if they were exploiting/ doing not good, they'll get reset.
            [Command("__resetuser")]
            public async Task ResetUserAsync() { }

            [Command("__taskmanager")]
            public async Task TaskManagerAsync() { }

            [Command("__shell")]
            public async Task CommandLineAsync() { }
        }
    }
}
