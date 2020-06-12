using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Drawing;
using Orikivo.Drawing.Encoding;
using Orikivo.Drawing.Graphics2D;
using Orikivo.Drawing.Graphics3D;
using Orikivo.Gaming;
using Orikivo.Desync;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Image = System.Drawing.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using SysColor = System.Drawing.Color;
using Discord.Rest;

namespace Orikivo
{

    public class Moderation : OriModuleBase<OriCommandContext>
    {

    }

    // Since Messy is a testing module, this doesn't need services.
    [Name("Messy")]
    [Summary("Commands that are under the works. Functionality is not to be expected.")]
    public class MessyModule : OriModuleBase<OriCommandContext>
    {
        private readonly DiscordSocketClient _client;
        //private readonly GameManager _gameManager;
        public MessyModule(DiscordSocketClient client)
        {
            _client = client;
            //_gameManager = manager;
        }

        public static Grid<ConwayCell> Pattern = ConwayRenderer.GetRandomPattern(128, 128);

        

        

        // TODO: Figure out what to display for the profile.
        //[Command("profile"), Alias("pf"), Priority(0)]
        //[Summary("Returns a brief summary of your profile.")]
        //[RequireUser(AccountHandling.ReadOnly)]

        // make generic events
        [Group("greetings")]
        public class GreetingsGroup : OriModuleBase<OriCommandContext>
        {
            [Command("")]
            [Summary("Shows the list of all greetings used for this guild."), RequireContext(ContextType.Guild)]
            public async Task GetGreetingsAsync(int page = 1)
            {
                await Context.Channel.SendMessageAsync($"{(Context.Server.Options.UseEvents ? "" : $"> Greetings are currently disabled.\n")}```autohotkey\n{(Context.Server.Options.Greetings.Count > 0 ? string.Join('\n', Context.Server.Options.Greetings.Select(x => $"[{Context.Server.Options.Events.IndexOf(x)}] :: {x.Message}")) : "There are currently no greetings set.")}```");
            }

            [Command("add")]
            [Access(AccessLevel.Inherit), RequireContext(ContextType.Guild)]
            [Summary("Adds a greeting to the collection of greetings for this guild.")]
            public async Task AddGreetingAsync([Remainder]string greeting)
            {
                try
                {
                    Context.Server.Options.Events.Add(new GuildEvent(EventType.UserJoin, greeting));
                    await Context.Channel.SendMessageAsync($"> Greeting **#{Context.Server.Options.Greetings.Count - 1}** has been included.");
                }
                catch (Exception e)
                {
                    await Context.Channel.CatchAsync(e);
                }
            }

            [Command("remove"), Alias("rm")]
            [Access(AccessLevel.Inherit)]
            [Summary("Removes the greeting at the specified index (zero-based)."), RequireContext(ContextType.Guild)]
            public async Task RemoveGreetingAsync(int index)
            {
                Context.Server.Options.Events.RemoveAt(index);
                // this will throw if outside of bounds
                await Context.Channel.SendMessageAsync($"> Greeting **#{index}** has been removed.");
            }

            [Command("clear")]
            [Access(AccessLevel.Owner)]
            [Summary("Clears all custom greetings written for this guild."), RequireContext(ContextType.Guild)]
            public async Task ClearGreetingsAsync()
            {
                Context.Server.Options.Events.RemoveAll(x => x.Type == EventType.UserJoin);
                await Context.Channel.SendMessageAsync($"> All greetings have been cleared.");
            }

            [Command("toggle")]
            [Access(AccessLevel.Owner)]
            [Summary("Toggles the ability to use greetings whenever a user joins."), RequireContext(ContextType.Guild)]
            public async Task ToggleGreetingsAsync()
            {
                Context.Server.Options.UseEvents = !Context.Server.Options.UseEvents;
                await Context.Channel.SendMessageAsync($"> **Greetings** {(Context.Server.Options.UseEvents ? "enabled" : "disabled")}.");
            }
        }

        [Command("defaultrole"), Priority(1)]
        [Access(AccessLevel.Owner), RequireContext(ContextType.Guild)]
        public async Task SetDefaultRoleAsync(SocketRole role)
        {
            Context.Server.Options.DefaultRoleId = role.Id;
            await Context.Channel.SendMessageAsync("The default role has been set.");
        }

        [Command("defaultrole"), Priority(0), RequireContext(ContextType.Guild)]
        public async Task GetDefaultRoleAsync()
        {
            await Context.Channel.SendMessageAsync($"The current default role id: `{Context.Server.Options.DefaultRoleId ?? 0}`");
        }

        [Command("mute")]
        [Access(AccessLevel.Owner)]
        [RequireBotPermission(ChannelPermission.ManageRoles), RequireContext(ContextType.Guild)]
        public async Task MuteUserAsync(SocketGuildUser user, double seconds)
        {
            if (!Context.Server.Options.MuteRoleId.HasValue)
            {
                RestRole muteRole = Context.Guild.CreateRoleAsync("Muted", new GuildPermissions(66560), null, false, false).Result;
                Context.Server.Options.MuteRoleId = muteRole.Id;
            }

            SocketRole role = Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value);
            if (role == null)
            {
                RestRole muteRole = Context.Guild.CreateRoleAsync("Muted", new GuildPermissions(66560), null, false, false).Result;
                Context.Server.Options.MuteRoleId = muteRole.Id;
            }

            if (Context.Server.HasMuted(user.Id))
            {
                Context.Server.Mute(user.Id, seconds);
                await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} has been muted for another {OriFormat.GetShortTime(seconds)}.");
                return;
            }

            await user.AddRoleAsync(Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value));
            Context.Server.Mute(user.Id, seconds);
            await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} has been muted for {OriFormat.GetShortTime(seconds)}.");

        }

        [Command("unmute")]
        [Access(AccessLevel.Owner)]
        [RequireBotPermission(ChannelPermission.ManageRoles), RequireContext(ContextType.Guild)]
        public async Task UnmuteUserAsync(SocketGuildUser user)
        {
            if (Context.Server.HasMuted(user.Id))
            {
                await user.RemoveRoleAsync(Context.Guild.GetRole(Context.Server.Options.MuteRoleId.Value));
                Context.Server.Unmute(user.Id);
                await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} has been unmuted.");
                return;
            }

            await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} is already unmuted.");
        }

        [Command("setrole")]
        [Access(AccessLevel.Owner)]
        [RequireContext(ContextType.Guild)]
        public async Task SetRoleAsync(SocketGuildUser user, SocketRole role)
        {
            if (user.Roles.Contains(role))
            {
                await Context.Channel.SendMessageAsync($"> {OriFormat.Username(user)} already has this role.");
                return;
            }

            await user.AddRoleAsync(role);
            await Context.Channel.SendMessageAsync($"> Gave {OriFormat.Username(user)} **{role.Name}**.");
        }

        // make a seperate field for the help menu with custom commands.
        [Command("customcommands"), RequireContext(ContextType.Guild)]
        public async Task GetCustomCommandsAsync(int page = 1)
        {
            if (Context.Server.Options.Commands.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"**Custom Commands**");
                sb.Append(string.Join(' ', Context.Server.Options.Commands.Select(x => $"`{x.Name}`")));
                await Context.Channel.SendMessageAsync(sb.ToString());
            }
            else
                await Context.Channel.SendMessageAsync($"There are currently no custom commands for this guild.");
        }

        // this is used to give the specified user the trust role
        //[Command("trust")]
        [Access(AccessLevel.Owner), RequireContext(ContextType.Guild)]
        public async Task TrustUserAsync(SocketGuildUser user) { }

        [Command("newcustomcommand")]
        [Access(AccessLevel.Inherit), RequireContext(ContextType.Guild)]
        public async Task SetCustomCommandAsync(string name, bool isEmbed, string imageUrl, [Remainder] string content = null)
        {
            GuildCommand command = new GuildCommand(Context.User, name);
            MessageBuilder message = new MessageBuilder(content, imageUrl);

            if (isEmbed)
                message.Embedder = Embedder.Default;

            command.Message = message;
            Context.Server.AddCommand(command);
            await Context.Channel.SendMessageAsync($"Your custom command (**{name}**) has been set.");
        }

        [Command("deletecustomcommand")]
        [Access(AccessLevel.Inherit), RequireContext(ContextType.Guild)]
        public async Task DeleteCustomCommandAsync(string name)
        {
            if (Context.Server.Options.Commands.Any(x => x.Name.ToLower() == name.ToLower()))
            {
                Context.Server.RemoveCommand(name);
                await Context.Channel.SendMessageAsync($"The custom command (**{name}**) has been deleted.");
            }
            else
                await Context.Channel.SendMessageAsync($"There aren't any custom commands in **{Context.Guild.Name}** that match '{name}'.");
        }

        [Command("closereport")]
        [Access(AccessLevel.Dev)]
        public async Task SetReportStatusAsync(int id, string reason = null)
        {
            if (Context.Global.Reports.Contains(id))
            {
                Context.Global.Reports.Close(id, reason);
                await Context.Channel.SendMessageAsync($"> **Report**#{id} has been closed.");
                return;
            }
            await Context.Channel.SendMessageAsync($"> I could not find any reports matching #{id}.");
        }

        

        [Command("singleset")]
        [Summary("Tests the SingleSet class.")]
        public async Task SingleSetTestAsync(int iterations = 10)
        {
            SingleSet f = new SingleSet(x => RandomProvider.Instance.Next(0, 100));
            SingleSet g = new SingleSet(x => RandomProvider.Instance.Next(0, 100));

            StringBuilder data = new StringBuilder();

            data.AppendLine("```Iterations:");

            for (int i = 0; i < iterations; i++)
            {
                data.AppendLine($"\nx = {i}");
                data.AppendLine($"f(x) = {f[i]}");
                data.AppendLine($"g(x) = {g[i]}");
                data.AppendLine($"(f+g)(x) = {(f + g)[i]}");
                data.AppendLine($"(f-g)(x) = {(f - g)[i]}");
                data.AppendLine($"(f/g)(x) = {(f / g)[i]}");
                data.AppendLine($"(f*g)(x) = {(f * g)[i]}");
                data.AppendLine($"(f%g)(x) = {(f % g)[i]}");
                data.AppendLine($"((f+g)-(f*g))(x) = {((f + g) - (f * g))[i]}");
            }
            data.Append("```");
            await Context.Channel.SendMessageAsync(data.ToString());
        }

        

        

        [Command("mapbits")]
        public async Task MapBitsAsync()
        {
            int width = RandomProvider.Instance.Next(1, 6);
            int height = RandomProvider.Instance.Next(1, 6);
            Grid<bool> from = new Grid<bool>(width, height, false);

            from.SetEachValue((x, y) => Randomizer.NextBool());

            byte[] bytes = Engine.CompressMap(from);
            Grid<bool> to = Engine.DecompressMap(width, height, bytes);

            StringBuilder maps = new StringBuilder();

            maps.AppendLine("**Original**:");
            maps.Append("```");
            maps.Append(from.ToString());
            maps.AppendLine("```");

            maps.AppendLine("**Compressed**:");
            maps.Append("```");
            maps.Append(string.Join(" ", bytes));
            maps.AppendLine("```");

            maps.AppendLine("**Restored**:");
            maps.Append("```");
            maps.Append(from.ToString());
            maps.AppendLine("```");

            await Context.Channel.SendMessageAsync(maps.ToString());
        }

        [Command("claim"), Priority(0)]
        [Summary("Displays a list of claimable **Merits**.")]
        [RequireUser]
        public async Task GetClaimableAsync()
        {
            if (Context.Account.Merits.Any(x => !x.Value.IsClaimed ?? false))
            {
                StringBuilder claimable = new StringBuilder();
                claimable.AppendLine($"**Claimable Merits:**");

                foreach(string id in Context.Account.Merits.Where(x => !x.Value.IsClaimed ?? false).Select(x => x.Key))
                {
                    Merit merit = Engine.GetMerit(id);

                    claimable.AppendLine($"`{id}` • **{merit.Name}**");
                }

                await Context.Channel.SendMessageAsync(claimable.ToString());
                return;
            }
            else
            {
                await Context.Channel.SendMessageAsync("There aren't any available merits to claim.");
            }
        }

        [Command("claim"), Priority(1)]
        [Summary("Claims a specified **Merit**.")]
        [RequireUser]
        public async Task ClaimMeritAsync(string id)
        {
            if (Context.Account.HasMerit(id))
            {
                if (Context.Account.Merits[id].IsClaimed ?? true)
                {
                    await Context.Channel.SendMessageAsync("This merit has either already been claimed or doesn't have a reward attached.");
                    return;
                }
                else
                {
                    Merit merit = Engine.GetMerit(id);

                    await Context.Channel.SendMessageAsync(merit.ClaimAndDisplay(Context.Account));
                }
            }
        }

        [Command("parseevent")]
        public async Task ParseEventAsync([Remainder] string content)
        {
            EventContext context = new EventContext(Context.Server, Context.Guild, Context.Guild.GetUser(Context.User.Id));

            StringBuilder result = new StringBuilder();

            result.AppendLine("```bf");
           

            result.AppendLine("Input:");
            result.AppendLine(content);

            result.AppendLine("```");

            result.AppendLine("**Output**:");
            result.Append(EventParser.Parse(content, context));

            await Context.Channel.SendMessageAsync(result.ToString());
        }

        
        /*
        [RequireUser(AccountHandling.ReadOnly)]
        [Command("balance"), Alias("money", "bal")]
        public async Task GetMoneyAsync()
        {
            StringBuilder values = new StringBuilder();

            values.AppendLine($"**Balance**: 💸 **{Context.Account.Balance.ToString("##,0.###")}**");
            values.AppendLine($"**Tokens**: 🏷️ **{Context.Account.TokenBalance.ToString("##,0.###")}**");
            values.AppendLine($"**Debt**: 📃 **{Context.Account.Debt.ToString("##,0.###")}**");

            await Context.Channel.SendMessageAsync(values.ToString());
        }*/

        [Command("testwaitfor")]
        public async Task WaitAsync(double seconds)
        {
            AsyncTimer timer = new AsyncTimer(TimeSpan.FromSeconds(seconds));

            timer.Start();
            await Task.WhenAny(timer.CompletionSource.Task);

            await Context.Channel.SendMessageAsync($"The timer has timed out. ({OriFormat.GetShortTime(timer.ElapsedTime.TotalSeconds)})");

        }

        [Command("chattest")]
        public async Task ChatTestAsync()
        {
            try
            {
                MessageCollector collector = new MessageCollector(Context.Client);
                SpriteBank bank = SpriteBank.FromDirectory("../assets/npcs/noname/");
                Npc test = new Npc
                {
                    Id = "npc0",
                    Name = "No-Name",
                    Personality = new Personality
                    {
                        Mind = MindType.Extravert,
                        Energy = EnergyType.Intuitive,
                        Nature = NatureType.Thinking,
                        Tactics = TacticType.Judging,
                        Identity = IdentityType.Assertive
                    },
                    Relations = new List<AffinityData>
                    {
                        new AffinityData("npc1", 0.2f)
                    },
                    Model = new NpcModel
                    {
                        Body = bank.GetSprite("noname_body"),
                        BodyOffset = new Point(20, 16),
                        Head = bank.GetSprite("noname_head"),
                        HeadOffset = new Point(28, 5),
                        FaceOffset = new Point(28, 5),
                        Reactions = new Dictionary<DialogTone, Sprite>
                        {
                            [DialogTone.Neutral] = bank.GetSprite("noname_neutral"),
                            [DialogTone.Happy] = bank.GetSprite("noname_happy"),
                            [DialogTone.Sad] = bank.GetSprite("noname_sad"),
                            [DialogTone.Confused] = bank.GetSprite("noname_confused"),
                            [DialogTone.Shocked] = bank.GetSprite("noname_shocked")
                        }
                    }
                };

                ChatHandler action = new ChatHandler(Context, test, Engine.GetPool("test"), PaletteType.Glass);

                MatchOptions options = new MatchOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = TimeSpan.FromSeconds(20),
                    Action = action
                };

                Func<SocketMessage, int, bool> filter = delegate (SocketMessage message, int index)
                {
                    return (message.Author.Id == Context.User.Id) && (message.Channel.Id == Context.Channel.Id);
                };

                await collector.MatchAsync(filter, options);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        }

        public static LinkedMessage Link;

        // create a LinkedMessage
        [Access(AccessLevel.Dev)]
        [Command("createlink")]
        public async Task CreateLinkAsync()
        {
            if (Link != null)
            {
                await Context.Channel.SendMessageAsync($"A link already exists. Its source is: **{Link.Source.Id}**.");
                return;
            }

            Link = LinkedMessage.Create(Context.Message, LinkDeleteHandling.Source, _client);

            await Context.Channel.SendMessageAsync("A link has been created on your previous message. When that is modified, all subscribers will be updated.");
        }

        [Access(AccessLevel.Dev)]
        [Command("links")]
        public async Task ViewLinksAsync()
        {
            if (Link?.Disposed ?? false)
                Link = null;

            if (Link == null)
            {
                await Context.Channel.SendMessageAsync("A link doesn't exist.");
                return;
            }

            StringBuilder links = new StringBuilder();

            links.AppendLine($"**Source**: {Link.Source.Id}");
            links.AppendLine($"> There are {Link.Subscribers.Count} {OriFormat.GetNounForm("subscriber", Link.Subscribers.Count)}.");

            Link.Subscribers.ForEach((x, i) => links.AppendLine($"{i}. `{x.Id}`"));

            await Context.Channel.SendMessageAsync(links.ToString());

        }

        [Access(AccessLevel.Dev)]
        [Command("addlink")]
        public async Task AddLinkAsync()
        {
            if (Link?.Disposed ?? false)
                Link = null;

            if (Link == null)
            {
                await Context.Channel.SendMessageAsync("A link doesn't exist.");
                return;
            }

            if (Context.Channel == null)
            {
                var dmChannel = await Context.Client.CurrentUser.GetOrCreateDMChannelAsync();
                await Link.CreateAsync(dmChannel);
            }
            else
                await Link.CreateAsync(Context.Channel);
        }

        [Access(AccessLevel.Dev)]
        [Command("removelink")]
        public async Task RemoveLinkAsync(ulong messageId)
        {
            if (Link?.Disposed ?? false)
                Link = null;

            if (Link == null)
            {
                await Context.Channel.SendMessageAsync("A link doesn't exist.");
                return;
            }

            if (Link.Remove(messageId))
                await Context.Channel.SendMessageAsync("Link removed.");
            else
                await Context.Channel.SendMessageAsync("Could not find a link with that ID.");
        }

        [Access(AccessLevel.Dev)]
        [Command("deletelink")]
        public async Task DeleteLinkAsync()
        {
            if (Link?.Disposed ?? false)
                Link = null;

            if (Link == null)
            {
                await Context.Channel.SendMessageAsync("A link doesn't exist.");
                return;
            }

            await Link.DeleteAsync();
        }

        [Command("testcollect")]
        [Summary("Tests the message collector system by attempting to filter messages starting with 'ok'.")]
        public async Task CollectAsync(double timeoutSeconds, bool resetTimeoutOnMatch = false, int? capacity = null)
        {
            MessageCollector collector = new MessageCollector(Context.Client);
            CollectionOptions options = new CollectionOptions { ResetTimeoutOnMatch = resetTimeoutOnMatch,
                Timeout = TimeSpan.FromSeconds(timeoutSeconds), Capacity = capacity, IncludeFailedMatches = false };

            FilterCollection c = await collector.CollectAsync(
                delegate (SocketMessage msg, FilterCollection matches, int i)
                {
                    return msg.Content.StartsWith("ok");
                }, options);
            StringBuilder sb = new StringBuilder();

            sb.Append($"The **MessageFilter** found **{c.Count}** successful {OriFormat.GetNounForm("match", c.Count)}. ({OriFormat.GetShortTime(collector.ElapsedTime?.TotalSeconds ?? 0)})");
            if (c.Count > 0)
                sb.Append(Format.Code($"{string.Join("\n", c.Select(x => $"[{x.Index}]: {x.Message.Content}"))}", "autohotkey"));

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        

        

        [Command("drawbitmap")]
        [Summary("Generates a **Bitmap** using unsafe and Parallel methods.")]
        public async Task DrawBitsAsync()
        {
            string path = $"../tmp/{Context.User.Id}_bits.png";
            Grid<SysColor> colors = new Grid<SysColor>(8, 8);
            colors.SetEachValue(delegate (int x, int y)
            {
                return GammaPalette.Default[(Gamma)x];
            });

            using (Bitmap bmp = GraphicsUtils.CreateRgbBitmap(colors.Values))
                bmp.Save(path, ImageFormat.Png);

            await Context.Channel.SendFileAsync(path);
        }

        [Command("screenshare"), Alias("ss")]
        public async Task ScreenshareAsync()
        {
            SocketGuildUser user = Context.Guild.GetUser(Context.User.Id);
            if (user.VoiceChannel == null)
                await Context.Channel.SendMessageAsync("> You aren't in a voice channel.");
            else
            {
                MessageBuilder mb = new MessageBuilder();
                string url = OriFormat.GetVoiceChannelUrl(Context.Guild.Id, user.VoiceChannel.Id);
                mb.Embedder = Embedder.Default;
                mb.Content = $"> **{Format.Url(user.VoiceChannel.Name, url)}** ({user.VoiceChannel.Users.Count}/{user.VoiceChannel.UserLimit?.ToString() ?? "∞"})";
                await Context.Channel.SendMessageAsync(mb.Build());
            }
        }

        





        [Command("testwhisper")]
        public async Task WhisperTestAsync()
        {
            await WhisperAsync(".....hi.");
            await ReplyAsync("Psst... I sent you something");
        }

        [Command("testroletoggle")]
        [Access(AccessLevel.Dev)]
        public async Task ToggleRoleTestAsync()
        {
            ulong roleId = 614686327019012098;
            IGuildUser user = Context.Guild.GetUser(Context.User.Id);
            IRole role = Context.Guild.GetRole(roleId);

            string message = "You shouldn't see this.";

            if (user.RoleIds.Contains(roleId))
            {
                await user.RemoveRoleAsync(role);
                message = "Removed role.";
            }
            else
            {
                await user.AddRoleAsync(role);
                message = "Added role.";
            }

            await Context.Channel.SendMessageAsync(message);
        }

        [Command("testpreset")]
        public async Task PresetTestAsync(bool useEmbed = false, bool hideUrl = false)
        {
            MessageBuilder msg = new MessageBuilder();
            msg.Content = "This is a message with content inside.";
            msg.Url = "https://steamcdn-a.akamaihd.net/steam/apps/730/header.jpg";
            msg.HideUrl = hideUrl;
            if (useEmbed)
                msg.Embedder = Embedder.Default;

            await Context.Channel.SendMessageAsync(msg.Build());
        }

        [Command("favorite"), Alias("fav"), Access(AccessLevel.Dev)]
        public async Task SetFavoriteAsync(ulong messageId)
        {
            IMessage message = Context.Channel.GetMessageAsync(messageId).Result;

            ulong favoriteChannelId = 654191904702988288;

            string content = "Message:\n";

            if (message.Attachments.Count > 0)
            {
                List<string> attachmentUrls = message.Attachments.Select(x => x.Url).ToList();
                content = string.Join('\n', attachmentUrls);
            }
            else
            {
                content = message.Content;
            }

            MessageBuilder msg = new MessageBuilder();
            msg.Content = content;
            

            await Context.Guild.GetTextChannel(favoriteChannelId).SendMessageAsync(msg.Build());
        }

        

        [Command("testrandomchoose")]
        public async Task ChooseTestAsync(int times = 8, bool allowRepeats = true)
        {
            times = times > 8 ? 8 : times < 1 ? 1 : times; // force bounds
            var values = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = new StringBuilder();
            result.AppendLine("I have picked the winners.");

            foreach (int value in Randomizer.ChooseMany(values, times, allowRepeats))
                result.AppendLine($"Selected: {value}");

            await Context.Channel.SendMessageAsync(result.ToString());
        }

        [Command("testrandomshuffle")]
        public async Task ShuffleTestAsync()
        {
            var values = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = new StringBuilder();
            result.AppendLine($"Old: {{ {string.Join(", ", values)} }}");
            List<int> shuffledValues = Randomizer.Shuffle(values).ToList();
            result.AppendLine($"OldEnsure: {{ {string.Join(", ", values)} }}");
            result.AppendLine($"New: {{ {string.Join(", ", shuffledValues)} }}");
            await Context.Channel.SendMessageAsync(result.ToString());
        }

        [Command("testdice")]
        public async Task DiceTestAsync()
        {
            Dice dice = Dice.Default;
            int result = Randomizer.Roll(dice);
            await Context.Channel.SendMessageAsync(result.ToString());
        }
    }
}
