using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Drawing;
using Orikivo.Desync;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Addons.Linking;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using SysColor = System.Drawing.Color;
using Discord.Rest;
using Orikivo.Canary;

namespace Orikivo.Modules
{
    // Since Messy is a testing module, this doesn't need services.
    [Name("Messy")]
    [Summary("Commands that are under the works. Functionality is not to be expected.")]
    public class MessyModule : BaseModule<DesyncContext>
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

        [Command("defaultrole"), Priority(1)]
        [Access(AccessLevel.Owner), RequireContext(ContextType.Guild)]
        public async Task SetDefaultRoleAsync(SocketRole role)
        {
            Context.Server.Config.DefaultRoleId = role.Id;
            await Context.Channel.SendMessageAsync("The default role has been set.");
        }

        [Command("defaultrole"), Priority(0), RequireContext(ContextType.Guild)]
        public async Task GetDefaultRoleAsync()
        {
            await Context.Channel.SendMessageAsync($"The current default role id: `{Context.Server.Config.DefaultRoleId ?? 0}`");
        }

        

        [Command("setrole")]
        [Access(AccessLevel.Owner)]
        [RequireContext(ContextType.Guild)]
        public async Task SetRoleAsync(SocketGuildUser user, SocketRole role)
        {
            if (user.Roles.Contains(role))
            {
                await Context.Channel.SendMessageAsync($"> **{user.Username}** already has this role.");
                return;
            }

            await user.AddRoleAsync(role);
            await Context.Channel.SendMessageAsync($"> Gave **{user.Username}** **{role.Name}**.");
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

        [Command("mapbits")]
        public async Task MapBitsAsync()
        {
            int width = RandomProvider.Instance.Next(1, 6);
            int height = RandomProvider.Instance.Next(1, 6);
            Grid<bool> from = new Grid<bool>(width, height, false);

            from.SetEachValue((x, y, z) => Randomizer.NextBool());

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

                    await Context.Channel.SendMessageAsync(MeritHandler.ClaimAndDisplay(Context.Account, merit));
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

        [Command("testwaitfor")]
        public async Task WaitAsync(double seconds)
        {
            AsyncTimer timer = new AsyncTimer(TimeSpan.FromSeconds(seconds));

            timer.Start();
            await Task.WhenAny(timer.CompletionSource.Task);

            await Context.Channel.SendMessageAsync($"The timer has timed out. ({Format.Counter(timer.ElapsedTime.TotalSeconds)})");

        }

        [Command("chattest")]
        public async Task ChatTestAsync()
        {
            try
            {
                MessageCollector collector = new MessageCollector(Context.Client);
                SpriteBank bank = SpriteBank.FromDirectory("../assets/npcs/noname/");
                Character test = new Character
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
                    Affinity = new List<AffinityData>
                    {
                        new AffinityData("npc1", 0.2f)
                    },
                    Model = new CharacterModel
                    {
                        Body = new AppearanceNode(bank.GetSprite("noname_body"), 20, 16),
                        Head = new AppearanceNode(bank.GetSprite("noname_head"), 28, 5),
                        DefaultFaceOffset = new Point(28, 5),
                        Expressions = new Dictionary<DialogTone, AppearanceNode>
                        {
                            [DialogTone.Neutral] = new AppearanceNode(bank.GetSprite("noname_neutral")),
                            [DialogTone.Happy] = new AppearanceNode(bank.GetSprite("noname_happy")),
                            [DialogTone.Sad] = new AppearanceNode(bank.GetSprite("noname_sad")),
                            [DialogTone.Confused] = new AppearanceNode(bank.GetSprite("noname_confused")),
                            [DialogTone.Shocked] = new AppearanceNode(bank.GetSprite("noname_shocked"))
                        }
                    }
                };

                var action = new ChatHandler(Context, test, Engine.GetTree("test"));

                var options = new SessionOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = TimeSpan.FromSeconds(20)
                };

                bool Filter(SocketMessage message, int index)
                {
                    return (message.Author.Id == Context.User.Id) && (message.Channel.Id == Context.Channel.Id);
                }

                await collector.RunSessionAsync(action, Filter, options);
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
            links.AppendLine($"> There are {Link.Subscribers.Count} {Format.TryPluralize("subscriber", Link.Subscribers.Count)}.");

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
                await Link.CloneAsync(dmChannel);
            }
            else
                await Link.CloneAsync(Context.Channel);
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

            MessageMatchCollection c = await collector.CollectAsync(
                delegate (SocketMessage msg, MessageMatchCollection matches, int i)
                {
                    return msg.Content.StartsWith("ok");
                }, options);
            StringBuilder sb = new StringBuilder();

            sb.Append($"The **MessageFilter** found **{c.Count}** successful {Format.TryPluralize("match", c.Count)}. ({Format.Counter(collector.ElapsedTime?.TotalSeconds ?? 0)})");
            if (c.Count > 0)
                sb.Append(Discord.Format.Code($"{string.Join("\n", c.Select(x => $"[{x.Index}]: {x.Message.Content}"))}", "autohotkey"));

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

            using (Bitmap bmp = ImageHelper.CreateRgbBitmap(colors.Values))
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
                string url = Format.GetVoiceChannelUrl(Context.Guild.Id, user.VoiceChannel.Id);
                mb.Embedder = Embedder.Default;
                mb.Content = $"> **{Discord.Format.Url(user.VoiceChannel.Name, url)}** ({user.VoiceChannel.Users.Count}/{user.VoiceChannel.UserLimit?.ToString() ?? "∞"})";
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
            var msg = new MessageBuilder();
            msg.Content = "This is a message with content inside.";
            msg.Url = new MessageUrl("https://steamcdn-a.akamaihd.net/steam/apps/730/header.jpg")
            {
                IsHidden = hideUrl
            };

            if (useEmbed)
                msg.Embedder = Embedder.Default;

            await Context.Channel.SendMessageAsync(msg.Build());
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
