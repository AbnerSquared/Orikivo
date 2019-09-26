using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    // alpha module begins here
    [Name("Misc")]
    [Summary("A group of commands that have no distinct category.")]
    public class AlphaModule : OriModuleBase<OriCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly OriLoggerService _logger;
        private readonly GameManager _gameManager;
        public AlphaModule(DiscordSocketClient client, CommandService commandService,
            OriLoggerService logger, GameManager gameManager)
        {
            _client = client;
            _commandService = commandService;
            _logger = logger;
            _gameManager = gameManager;
        }
        
        [RequireGuild]
        [Command("lobby2")]
        [Summary("test the new lobby system")]
        
        public async Task TestLobbyAsync()
        {
            Game game = await _gameManager.CreateGameAsync(Context, new LobbyConfig());
            // get the first receiver.
            await Context.Channel.SendMessageAsync($"New lobby created. {game.Receivers[0].Mention}");
        }

        [Command("testdisplay")]
        [Summary("Test the new display overhaul.")]
        public async Task TestEventAsync()
        {
            // display config rework,
            // make the display update due to events
            // in this case, it makes the game system activate
            // due to called events, as opposed to being on the entire time

            StringNode node = new StringNode();
            node.Content = "This is a node.";
            StringNodeGroup group = new StringNodeGroup();
            StringNode groupNode = new StringNode();
            groupNode.Content = "This is a node within a group.";
            StringNode groupNode2 = new StringNode();
            groupNode2.Content = "This is also a node within a group.";
            group.AddNode(groupNode);
            group.AddNode(groupNode2);

            ImmutableDisplay display = new ImmutableDisplay(new List<StringNode> { node }, new List<StringNodeGroup> { group });
            display.Update("New Display");
            // test node updates, node group value limits
            // node page swapping


            await Context.Channel.SendMessageAsync(display.Content);
        }

        [Command("color")]
        [Summary("New color object testing.")]
        public async Task ColorAsync()
        {
            Color c = new Color(100, 100, 100);
            OriColor oriC = (OriColor)c;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"```bf");
            sb.AppendLine($"Discord.Color.RawValue == {c.RawValue}");
            sb.AppendLine($"OriColor.Value == {oriC.Value}");
            sb.AppendLine($"Discord.Color.R == {c.R}\nDiscord.Color.G == {c.G}\nDiscord.Color.B == {c.B}");
            sb.AppendLine($"OriColor.A == {oriC.A}\nOriColor.R == {oriC.R}\nOriColor.G == {oriC.G}\nOriColor.B == {oriC.B}");
            sb.AppendLine($"Discord.Color.ToString == {c.ToString()}");
            sb.AppendLine($"OriColor.ToString == {oriC.ToString()}");
            sb.AppendLine($"```");

            await Context.Channel.SendMessageAsync(sb.ToString());
        }


        [Group("greetings")]
        public class GreetingsGroup : OriModuleBase<OriCommandContext>
        {
            public GreetingsGroup() {}
            [RequireGuild]
            [Command("")]
            [Summary("Shows the list of all greetings used for this guild.")]
            public async Task GetGreetingsAsync(int page = 1)
            {
                await Context.Channel.SendMessageAsync($"{(Context.Server.Options.AllowGreeting ? "" : $"> Greetings are currently disabled.\n")}```autohotkey\n{string.Join('\n', Context.Server.Options.Greetings.Select(x => $"[{Context.Server.Options.Greetings.IndexOf(x)}] :: {x.Frame}"))}```");
            }
            [RequireGuild]
            [Command("add")]
            [BindTo(TrustLevel.Inherit)]
            [Summary("Adds a greeting to the collection of greetings for this guild.")]
            public async Task AddGreetingAsync([Remainder]string greeting)
            {
                try
                {
                    Context.Server.Options.Greetings.Add(new GuildGreeting { Frame = greeting });
                    await Context.Channel.SendMessageAsync($"> Greeting **#{Context.Server.Options.Greetings.Count - 1}** has been included.");
                }
                catch(Exception e)
                {
                    await Context.Channel.CatchAsync(e);
                }
            }
            [RequireGuild]
            [Command("remove"), Alias("rm")]
            [BindTo(TrustLevel.Inherit)]
            [Summary("Removes the greeting at the specified index (zero-based).")]
            public async Task RemoveGreetingAsync(int index)
            {
                Context.Server.Options.Greetings.RemoveAt(index);
                await Context.Channel.SendMessageAsync($"> Greeting **#{index}** has been removed.");
            }
            [RequireGuild]
            [Command("clear")]
            [BindTo(TrustLevel.Owner)]
            [Summary("Clears all custom greetings written for this guild.")]
            public async Task ClearGreetingsAsync()
            {
                Context.Server.Options.Greetings = OriGuildOptions.Default.Greetings;
                await Context.Channel.SendMessageAsync($"> All greetings have been reset.");
            }
            [RequireGuild]
            [Command("toggle")]
            [BindTo(TrustLevel.Owner)]
            [Summary("Toggles the ability to use greetings whenever a user joins.")]
            public async Task ToggleGreetingsAsync()
            {
                Context.Server.Options.AllowGreeting = !Context.Server.Options.AllowGreeting;
                await Context.Channel.SendMessageAsync($"> **Greetings** {(Context.Server.Options.AllowGreeting ? "enabled" : "disabled")}.");
            }
        }

        [RequireGuild]
        [Command("defaultrole"), Priority(1)]
        [BindTo(TrustLevel.Owner)]
        public async Task SetDefaultRoleAsync(SocketRole role)
        {
            Context.Server.Options.DefaultRoleId = role.Id;
            await Context.Channel.SendMessageAsync("The default role has been set.");
        }

        [RequireGuild]
        [Command("defaultrole"), Priority(0)]
        public async Task GetDefaultRoleAsync()
        {
            await Context.Channel.SendMessageAsync($"The current default role id: `{Context.Server.Options.DefaultRoleId ?? 0}`");
        }

        [RequireGuild]
        [Command("mute")]
        [BindTo(TrustLevel.Owner)]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        public async Task MuteUserAsync(SocketGuildUser user, double seconds)
        {
            if (!Context.Server.Options.MuteRoleId.HasValue)
            {
                RestRole muteRole = Context.Guild.CreateRoleAsync("Muted", new GuildPermissions(66560)).Result;
                Context.Server.Options.MuteRoleId = muteRole.Id;
            }

            SocketRole role = Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value);
            if (role == null)
            {
                RestRole muteRole = Context.Guild.CreateRoleAsync("Muted", new GuildPermissions(66560)).Result;
                Context.Server.Options.MuteRoleId = muteRole.Id;
            }
            role = Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value);


            if (Context.Server.HasMuted(user.Id))
            {
                Context.Server.Mute(user.Id, seconds);
                await Context.Channel.SendMessageAsync($"> {OriFormat.GetUserName(user)} has been muted for another {OriFormat.GetShortTime(seconds)}.");
                return;
            }

            await user.AddRoleAsync(Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value));
            Context.Server.Mute(user.Id, seconds);
            await Context.Channel.SendMessageAsync($"> {OriFormat.GetUserName(user)} has been muted for {OriFormat.GetShortTime(seconds)}.");

        }

        [RequireGuild]
        [Command("unmute")]
        [BindTo(TrustLevel.Owner)]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        public async Task UnmuteUserAsync(SocketGuildUser user)
        {
            if (Context.Server.HasMuted(user.Id))
            {
                await user.RemoveRoleAsync(Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value));
                Context.Server.Unmute(user.Id);
                await Context.Channel.SendMessageAsync($"> {OriFormat.GetUserName(user)} has been unmuted.");
                return;
            }

            await Context.Channel.SendMessageAsync($"> {OriFormat.GetUserName(user)} is already unmuted.");
        }

        [RequireGuild]
        [Command("setrole")]
        [BindTo(TrustLevel.Owner)]
        [RequirePermissions(GuildPermission.ManageRoles)]
        public async Task SetRoleAsync(SocketGuildUser user, SocketRole role)
        {
            if (user.Roles.Contains(role))
            {
                await Context.Channel.SendMessageAsync($"> {OriFormat.GetUserName(user)} already has this role.");
                return;
            }

            await user.AddRoleAsync(role);
            await Context.Channel.SendMessageAsync($"> Gave {OriFormat.GetUserName(user)} **{role.Name}**.");
        }



        [Command("reply")]
        [BindTo(TrustLevel.Dev)]
        public async Task InvokeReactiveAsync()
        {
            await ReplyAsync("Just type anything:");
            SocketMessage response = await GetMessageAsync();
            if (response != null)
                await Context.Channel.SendMessageAsync($"Ok, cool.");
            else
                await Context.Channel.SendMessageAsync("Alright, I guess don't.");
        }

        [RequireGuild]
        // make a seperate field for the help menu with custom commands.
        [Command("customcommands")]
        public async Task GetCustomCommandsAsync(int page = 1)
        {
            if (Context.Server.CustomCommands.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"**Custom Commands**");
                sb.Append(string.Join(' ', Context.Server.CustomCommands.Select(x => $"`{x.Name}`")));
                await Context.Channel.SendMessageAsync(sb.ToString());
            }
            else
                await Context.Channel.SendMessageAsync($"There are currently no custom commands for this guild.");
        }

        [RequireGuild]
        // this is used to give the specified user the trust role
        [Command("trust")]
        [BindTo(TrustLevel.Owner)]
        public async Task TrustUserAsync(SocketGuildUser user)
        {

        }

        [RequireGuild]
        [Command("newcustomcommand")]
        [BindTo(TrustLevel.Inherit)]
        public async Task SetCustomCommandAsync(string name, bool isEmbed, string imageUrl, [Remainder] string content = null)
        {
            CustomGuildCommand command = new CustomGuildCommand(name);
            CustomCommandMessage msg = new CustomCommandMessage(imageUrl, content, isEmbed ? OriEmbedOptions.Default : null);
            command.Message = msg;
            Context.Server.AddCommand(command);
            await Context.Channel.SendMessageAsync($"Your custom command (**{name}**) has been set.");
        }

        [RequireGuild]
        [Command("deletecustomcommand")]
        [BindTo(TrustLevel.Inherit)]
        public async Task DeleteCustomCommandAsync(string name)
        {
            if (Context.Server.CustomCommands.Any(x => x.Name.ToLower() == name.ToLower()))
            {
                Context.Server.TryRemoveCommand(name);
                await Context.Channel.SendMessageAsync($"The custom command (**{name}**) has been deleted.");
            }
            else
                await Context.Channel.SendMessageAsync("There aren't any custom commands in this guild with that name.");
        }

        [Command("setreport")]
        [BindTo(TrustLevel.Dev)]
        public async Task SetReportStatusAsync(int index, ReportStatus status)
        {
            if (Context.Global.HasReport(index))
            {
                if (Context.Global.UpdateReport(index, status))
                {
                    await Context.Channel.SendMessageAsync($"Report#{index} has been updated.");
                    return;
                }
            }
            await Context.Channel.SendMessageAsync("There were no valid reports with the specified index.");
        }

        [Command("report")]
        public async Task GetReportAsync(int index)
        {
            if (Context.Global.HasReport(index))
            {
                await Context.Channel.SendMessageAsync(Context.Global.GetReport(index).GetInfo());
                return;
            }
            await Context.Channel.SendMessageAsync("There were no valid reports with the specified index.");
        }

        [Command("reports")]
        public async Task ReportsAsync()
        {
            await Context.Channel.SendMessageAsync($"{Context.Global.ReportIndex}");
        }

        [Command("newreport")]
        [RequireUserAccount]
        [Cooldown(300)]
        [ArgSeparatorChar(',')]
        public async Task ReportAsync(string context, ReportFlag level, string title, string content, string imageUrl = null)
        {
            ContextInfo ctx = ContextInfo.Parse(content);
            ContextSearchResult result = new OriHelpService(_commandService).Search(context);
            if (result.IsSuccess)
            {
                if (result.ResultType.HasValue)
                    if (result.ResultType.Value == ContextInfoType.Overload)
                    {
                        Context.Global.AddReport(Context.Account, result.Overload, level, new ReportBodyInfo(title, content, imageUrl));
                        await Context.Channel.SendMessageAsync("Your report has been submitted.");
                        return;
                    }
            }
            await Context.Channel.SendMessageAsync("The command you specified does not exist.");
        }

        [Group("gimi")]
        public class GimiGroup : OriModuleBase<OriCommandContext>
        {
            [Command("")]
            [Cooldown(10)]
            [RequireUserAccount]
            [Summary("A **CasinoType** activity that randomly offers a reward value.")]
            public async Task GimiAsync()
            {
                Gimi gimi = new Gimi(Context.Account);
                int returns = gimi.Get();
                Context.Account.SetStat(returns > 0 ? GimiStat.CurrentLossStreak : GimiStat.CurrentWinStreak, 0);
                Context.Account.UpdateStat(returns > 0 ? GimiStat.CurrentWinStreak : GimiStat.CurrentLossStreak, 1);
                Context.Account.UpdateStat(returns > 0 ? GimiStat.TimesWon : GimiStat.TimesLost, 1);
            
                await Context.Channel.SendMessageAsync($"You got: {returns}");
            }

            [Command("risk"), Priority(0)]
            [RequireUserAccount]
            public async Task GetGimiRiskAsync()
            {
                await Context.Channel.SendMessageAsync($"> Your **Risk** is currently set to **{Context.Account.Gimi.Risk}**%.");
            }

            [Command("risk"), Priority(1)]
            [RequireUserAccount]
            public async Task SetGimiRiskAsync(int risk)
            {
                Context.Account.Gimi.SetRisk(risk);
                await Context.Channel.SendMessageAsync($"> Your **Risk** has been set to **{Context.Account.Gimi.Risk}**%.");
            }

            [Command("stats")]
            [RequireUserAccount]
            public async Task GetGimiStatsAsync()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"> Gimi");
                sb.AppendLine($"> **Stats**");
                sb.AppendLine($"**{Context.Account.GetStat(GimiStat.TimesWon)}+{Context.Account.GetStat(GimiStat.TimesWonGold)}**W : **{Context.Account.GetStat(GimiStat.TimesLost)}+{Context.Account.GetStat(GimiStat.TimesLostCursed)}**L : **{Context.Account.GetStat(GimiStat.TimesPlayed)}**P");
                sb.AppendLine($"${Context.Account.GetStat(GimiStat.TotalAmountWon)} Earned : ${Context.Account.GetStat(GimiStat.TotalAmountLost)} Lost");
                sb.AppendLine($"Longest Win Streak: {Context.Account.GetStat(GimiStat.LargestWinStreakLength)} (${Context.Account.GetStat(GimiStat.LargestWinStreakAmount)}");
                sb.AppendLine($"Longest Gold Streak: {Context.Account.GetStat(GimiStat.LargestGoldStreakLength)}");
                sb.AppendLine($"Longest Loss Streak: {Context.Account.GetStat(GimiStat.LargestLossStreakLength)} (${Context.Account.GetStat(GimiStat.LargestLossStreakAmount)})");
                sb.AppendLine($"Longest Curse Streak: {Context.Account.GetStat(GimiStat.LargestCurseStreakLength)}");

                await Context.Channel.SendMessageAsync(sb.ToString());
            }

            [Command("earn"), Priority(0)]
            [RequireUserAccount]
            public async Task SetGimiEarnAsync()
            {
                await Context.Channel.SendMessageAsync($"> Your **Earn** is currently set to {Context.Account.Gimi.Earn}.");
            }

            [Command("earn"), Priority(1)]
            [RequireUserAccount]
            public async Task SetGimiEarnAsync(int earn)
            {
                Context.Account.Gimi.SetEarn(earn);
                await Context.Channel.SendMessageAsync($"> Your **Earn** has been set to **{Context.Account.Gimi.Earn}**.");
            }

            [Command("slots")]
            [RequireUserAccount]
            public async Task GetGimiSlotsAsync()
            {
                await Context.Channel.SendMessageAsync($">>> **Gold**#{Context.Account.Gimi.GoldSlot}\n**Curse**#{Context.Account.Gimi.CurseSlot}");
            }

            [Command("clear")]
            [RequireUserAccount]
            [Cooldown(86400)]
            public async Task RegenGimiSlotsAsync()
            {
                Context.Account.Gimi.ClearSlots();
                await Context.Channel.SendMessageAsync("> **Gimi** slots cleared.");
            }
        }

        [Command("help"), Alias("h")]
        [Summary("Gets the underlying **DisplayInfo** for a specified object.")]
        public async Task GetHelpInfoAsync(
            [Remainder][Summary("The **ContextInfo** for the **OriHelpService** to utilize.")]string context = null
            )
        {
            try
            {
                OriHelpService helper = new OriHelpService(_commandService);
                await Context.Channel.SendMessageAsync(helper.Search(context).GetResultInfo() ?? helper.GetDefaultInfo());
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("lobbies"), Alias("lbl")]
        [Summary("Gets all active entities of **OriLobbyInvoker**.")]
        [RequireUserAccount]
        public async Task ShowLobbiesAsync([Summary("The result page to be on.")]int page = 1)
        {
            List<string> lobbies = new List<string>();
            foreach (OriLobbyInvoker lobby in Context.Global.Lobbies)
            {
                lobbies.Add(lobby.Summary);
            }

            string result = string.Join("\n``` ```\n", lobbies);
            if (string.IsNullOrWhiteSpace(result))
            {
                result = "**Null.**\nThere are currently no open lobbies.";
            }

            await Context.Channel.SendMessageAsync(result);
        }

        [Command("joinlobby"), Alias("jlb")]
        [Summary("Join an active **OriLobbyInvoker**.")]
        [RequireUserAccount]
        public async Task JoinLobbyAsync([Summary("The identity key for a specific **OriLobbyInvoker**.")]string id)
        {
            if (Context.Global.HasUserInLobby(Context.Account.Id))
            {
                await Context.Channel.SendMessageAsync($"**Wait!**\nYou are already in a lobby.");
                return;
            }
            if (!Context.Global.Lobbies.Any(x => x.Id == id))
            {
                await Context.Channel.SendMessageAsync($"**Empty.**\nThere no lobbies at #**{id}**.");
                return;
            }
            else
            {
                OriLobbyInvoker lobby = Context.Global.Lobbies.First(x => x.Id == id);
                if (lobby.HasUser(Context.User.Id))
                {
                    await Context.Channel.SendMessageAsync($"**Wait!**\nYou are already in this lobby.");
                    return;
                }
                await lobby.AddUserAsync(Context);
                await Context.Channel.SendMessageAsync($"**Success!**\nYou have joined {lobby.Name}. [{lobby.Receivers.First(x => x.Id == Context.Guild.Id).Mention}]");
            }
        }

        [Command("createlobby"), Alias("crlb")]
        [Summary("Create an **OriLobbyInvoker** for a specified **OriGameType**.")]
        [RequireUserAccount]
        public async Task StartLobbyAsync([Summary("The **OriGameType** to use for an **OriLobbyInvoker**.")]GameMode gameType)
        {
            if (Context.Global.HasUserInLobby(Context.Account.Id))
            {
                await Context.Channel.SendMessageAsync($"**Wait!**\nYou are already in a lobby.");
                return;
            }
            try
            {
                LobbyConfig config = new LobbyConfig();

                config.Name = $"{Context.User.Username}'s Lobby";
                config.Mode = GameMode.Werewolf;
                config.Privacy = LobbyPrivacy.Public;
                
                OriLobbyInvoker lobby = new OriLobbyInvoker(_client, Context, new ReceiverConfig(), config);
                Context.Global.AddLobby(lobby);
                await Context.Channel.SendMessageAsync($"**Success!**\n{lobby.Name} has been created. [{lobby.Receivers.First(x => x.Id == Context.Guild.Id).Mention}]");
                await lobby.StartAsync(Context);
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("profile"), Alias("pf")]
        [Summary("Gets the **OriUser** object from yourself.")]
        [RequireUserAccount]
        public async Task GetUserTestAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Account.GetDisplay(Context.Account.Options.DisplayFormat));
        }

        [Command("guildprofile"), Alias("server", "gpf")]
        [Summary("Gets the **OriGuild** object for the current **SocketGuild**.")]
        [RequireUserAccount]
        public async Task GetGuildTestAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Server.GetDisplay(Context.Account.Options.DisplayFormat));
        }

        [Command("stats")]
        [RequireUserAccount]
        public async Task GetStatsAync()
        {
            await Context.Channel.SendMessageAsync($"**Commands Used**: {Context.Account.GetStat(Stat.CommandsUsed)}");
        }

        [Command("displayformat"), Alias("dispfmt"), Priority(0)]
        [Summary("Shows your current **EntityDisplayFormat**.")]
        [RequireUserAccount]
        public async Task GetDisplayFormatAsync()
        {
            await Context.Channel.SendMessageAsync(Context.Account.Options.DisplayFormat.ToString());
        }

        [Command("displayformat"), Alias("dispfmt"), Priority(1)]
        [Summary("Set your **EntityDisplayFormat** to the specified type.")]
        [RequireUserAccount]
        public async Task SetDisplayFormatAsync([Summary("The new **EntityDisplayFormat** to set.")]EntityDisplayFormat format)
        {
            Context.Account.Options.DisplayFormat = format;
            await Context.Channel.SendMessageAsync("Display format set.");
        }
    }
}
