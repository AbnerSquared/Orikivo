using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orikivo.Systems.Presets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Modules
{
    // a private module that works only with the mod prefix.

    [Name("Moderation")]
    [Summary("Build and edit server defaults.")]
    [DontAutoLoad]
    public class ModerationModule : ModuleBase<OrikivoCommandContext>
    {
        private readonly CommandService _service;
        private readonly DiscordSocketClient _socket;
        private readonly IConfigurationRoot _config;

        public ModerationModule(CommandService service,
            IConfigurationRoot config,
            DiscordSocketClient socket)
        {
            _service = service;
            _config = config;
            _socket = socket;
        }

        //

        /*
            to-do:
            set-up mod roles
            set-up administrator roles
            set-up assistant roles
            set-up dj roles
            toggle self-assignable roles
            set-up muted roles
            


            default constructs:
            build new invite
            build new roles
            build new emojis
            build new channels
            build new categories
            place channels into categories

            DiscordSocketClient


            later on:
            edit channels
            edit roles
            edit categories

            be able to hackban
            be able to ban
            be able to softban
            be able to kick
            be able to wipe a user entirely
            be able to prevent a user
                from joining guilds that you're the owner of
                and that has the bot.

            be able to set-up blacklisted words
            be able to block links
            be able to block advertising
            be able to block spam
            be able to block caps
            be able to limit emojis

            be able to edit searches with
                - starts with WORD
                - contains WORD
                - exactly matches Word
                
            be able to limit pages to a healthy size
                that works for all list generators.

                - make sure listing starts indexing at
                a specified page, if it can.
                and generate until either
                no elements are left, or it reaches the page
                list limit.

                ^ this would help heavily with lists
                that contain over 1000s of entities

            statistics for anything
                - users
                - roles
                - text channels
                - voice channels
                - categories
                - servers
                - accounts
                - orikivo
                - dev server

            be able to view server verification levels

            None | ヽ（ ﾟヮﾟ）ﾉ | No verification needed
            Low | (ノ゜-゜)ノ | Needs an verified e-mail.
            Medium | \(°□°)/ | Must wait 5 min. after joining
            High | (╯°□°）╯︵ ┻━┻ | must wait 10 min. after joining.
            Extreme | ┻━┻彡 ヽ(ಠ益ಠ)ノ彡┻━┻ | must require phone number.
         */

        // assign yourself a role from the assignable role list.
        //[Command("assignrole")]
        public async Task AssignRoleResponseAsync()
        {

        }

        // toggle current roles as self assignable.
        //[Command("selfrole")]
        public async Task SelfRoleResponseAsync()
        {

        }

        //[Command("muterole")]
        public async Task SetMuteRoleResponseAsync()
        {

        }

        //[Command("mute")]
        public async Task MuteResponseAsync()
        {

        }


        // adds the converted id into a blacklist.
        // any response from any of the following:
        // user
        // role
        // channel
        // they will not be able to execute commands.
        //[Command("ignore")]
        public async Task IgnoreAnyResponseAsync()
        {

        }


        // set a channel as the verification channel. only when authenticate is enabled.
        //[Command("verificationchannel")]
        public async Task VerificationChannelResponseAsync()
        {

        }

        // toggle authentication for the server
        // if enabled, all new users will be assigned an unverified role, which prevents any communication
        // except the default verification channel. until they enter the captcha, they will be locked out.
        // if the captcha is correct, they will no longer have the unverified role, and 
        // will be assigned the default role

        // required permissions

        // OWNER ONLY.
        //[Command("authenticate")]
        public async Task AuthenticateServerResponseAsync()
        {

        }

       // [Command("defaultrole")]
        //[Summary("Default role for new users.")]
        public async Task BuildRoleAsync([Remainder]string roleContext = null)
        {
            //morality => subRole.subRole
            //morality => all
        }
    }
}
