using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orikivo.Drawing;
using System.Drawing;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Color = Discord.Color;
using SysColor = System.Drawing.Color;
using System.IO;
using Image = System.Drawing.Image;
using System.Text.RegularExpressions;
using Orikivo.Ascii;

namespace Orikivo
{
    // Since Messy is a testing module, this doesn't need services.
    [Name("Messy")]
    [Summary("Commands that are under the works. Functionality is not to be expected.")]
    public class MessyModule : OriModuleBase<OriCommandContext>
    {
        private readonly GameManager _gameManager;
        public MessyModule(GameManager manager)
        {
            _gameManager = manager;
        }

        private List<Stream> DrawTextFrames(params string[] texts)
        {
            if (texts.Length == 0)
                throw new ArgumentNullException("At least one text must be written.");

            List<Stream> frames = new List<Stream>();
            FontFace font = OriJsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            CanvasProperties canvasProperties = new CanvasProperties { UseNonEmptyWidth = false, Padding = new Padding(2), BackgroundColor = new OriColor(0x0C525F) };
            string framePath = $"../tmp/{Context.User.Id}_frame{{0}}.png";

            using (PixelGraphics poxel = new PixelGraphics(OriJsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new DynamicArrayJsonConverter<char>())))
            {
                poxel.DefaultCanvasProperties = canvasProperties;
                for (int i = 0; i < texts.Length; i++)
                {
                    // string outputPath = string.Format(framePath, i);

                    MemoryStream stream = new MemoryStream();
                    using (Bitmap frame = poxel.DrawString(texts[i], font, OriColor.OriGreen, options: canvasProperties))
                    {
                        frame.Save(stream, ImageFormat.Png);
                        // BitmapUtils.Save(frame, outputPath, ImageFormat.Png);
                    }


                    frames.Add(stream);

                    //frames.Add(outputPath);
                }
            }

            return frames;
        }

        [Command("drawanimr")]
        [Summary("Creates a GIFASCII animation using the AsciiEngine.")]
        public async Task DrawRenderAsync()
        {
            using (var engine = new AsciiEngine(12, 12))
            {
                engine.CurrentGrid.CreateAndAddObject("DVD", '\n', 0, 0, 0, GridCollideMethod.Reflect, new AsciiVector(1, 0, 0, 0));
                engine.CurrentGrid.CreateAndAddObject("DV2", '\n', 0, 1, 0, GridCollideMethod.Reflect, new AsciiVector(0.5f, 0, 0, 0));
                engine.CurrentGrid.CreateAndAddObject("DV3", '\n', 0, 2, 0, GridCollideMethod.Reflect, new AsciiVector(0.25f, 0, 0, 0));
                string[] frames = engine.GetFrames(0, 9, 1);
                await DrawAnimAsync(150, frames);
            }
        }

        /// <summary>
        /// Creates a GIFASCII animation from a given .txt file with an optional specified delay.
        /// </summary>
        /// <param name="delay">The length of delay per frame (in milliseconds).</param>
        [Command("drawanimf")]
        [Summary("Creates a GIFASCII animation from a given .txt file with an optional specified delay (in milliseconds).")]
        public async Task DrawAnimAsync([Summary("The length of delay per frame (in milliseconds).")]int delay = 150)
        {
            if (Context.Message.Attachments.Count == 0)
                throw new Exception("You need to send a .txt file containing the frames!");

            if (!Context.Message.Attachments.Any(x => EnumUtils.GetUrlType(x.Filename) == UrlType.Text))
                throw new Exception("You need to send a .txt file containing the frames!");

            Attachment textFile = Context.Message.Attachments.Where(x => EnumUtils.GetUrlType(x.Filename) == UrlType.Text).First();
            OriWebResult urlResult = await new OriWebClient().RequestAsync(textFile.Url);

            string[] urlFrames = ParseFrames(urlResult.RawContent);

            if (urlFrames.Length == 0)
                throw new Exception("There weren't any proper frames specified.");

            await DrawAnimAsync(delay, urlFrames);
        }

        private string[] ParseFrames(string rawContent)
        {
            List<string> frames = new List<string>();
            Regex regex = new Regex("([\"\'])(?:(?=(\\\\?))\\2[\\s\\S])*?\\1");
            MatchCollection matches = regex.Matches(rawContent);
            foreach(Match match in matches)
            {
                Console.WriteLine($"Match!:\n{string.Join(", ", match.Value.Select(x => $"\'{x}\'"))}");
                if (match.Success)
                {
                    string frame = match.Value.Trim('\"', '\'');
                    frame = frame.Replace("\r", "");
                    frames.Add(frame);
                }

            }

            return frames.ToArray();
        }

        [Command("drawanim")]
        public async Task DrawAnimAsync(params string[] strings)
            => await DrawAnimAsync(150, strings);

        [Command("drawanimd")]
        public async Task DrawAnimAsync(int millisecondDelay, params string[] strings)
        {
            string gifPath = $"../tmp/{Context.User.Id}_anim.gif";
            FontFace font = OriJsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            CanvasProperties canvasProperties = new CanvasProperties { UseNonEmptyWidth = false, Padding = new Padding(2), BackgroundColor = new OriColor(0x0C525F) };
            // animate a, b  

            MemoryStream gifStream = new MemoryStream();
            using (GifEncoder encoder = new GifEncoder(gifStream))
            {
                encoder.FrameDelay = TimeSpan.FromMilliseconds(millisecondDelay);

                /*
                    "o_____"
                    "_o____"
                    "__o___"
                    "___o__"
                    "____o_"
                    "_____o"
                    "____o_"
                    "___o__"
                    "__o___"
                    "_o____"
                */

                /*
                    "-"
                    "/"
                    "|"
                    "\\"
                */

                //foreach (string frame in DrawTextFrames(strings))
                //    encoder.EncodeFrame(Image.FromFile(frame));

                foreach (Stream frame in DrawTextFrames(strings))
                    using (frame)
                        encoder.EncodeFrame(Image.FromStream(frame));
            }

            gifStream.Position = 0;
            Image gifResult = Image.FromStream(gifStream);
            gifResult.Save(gifPath, ImageFormat.Gif);
            gifStream.Dispose();

            await Context.Channel.SendFileAsync(gifPath);
            //await Context.Channel.SendFileAsync(gifStream, "gif_test.gif");
        }

        [Command("draw")]
        [Summary("Draws a pixelated string.")]
        public async Task DrawTextAsync([Remainder]string text)
        {
            string path = $"../tmp/{Context.User.Id}_text.png";
            FontFace font = OriJsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            // new OutlineProperties(1, new OriColor(0x44B29B)): Too taxing on performance as of now
            using (PixelGraphics poxel = new PixelGraphics(OriJsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new DynamicArrayJsonConverter<char>())))
                using (Bitmap bmp = poxel.DrawString(text, font, OriColor.OriGreen, null, new CanvasProperties { UseNonEmptyWidth = true, Padding = new Padding(2), BackgroundColor = new OriColor(0x0C525F) }))
                    BitmapUtils.Save(bmp, path, ImageFormat.Png);

            await Context.Channel.SendFileAsync(path, $"Draw `{text}`");
        }

        [Command("drawmono")]
        [Summary("Draws a pixelated string as monospaced text.")]
        public async Task DrawMonoTextAsync([Remainder]string text)
        {
            string path = $"../tmp/{Context.User.Id}_text.png";
            FontFace font = OriJsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            // new OutlineProperties(1, new OriColor(0x44B29B)): Too taxing on performance as of now
            using (PixelGraphics poxel = new PixelGraphics(OriJsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new DynamicArrayJsonConverter<char>())))
            using (Bitmap bmp = poxel.DrawString(text, font, OriColor.OriGreen, null, new CanvasProperties { UseNonEmptyWidth = false, Padding = new Padding(2), BackgroundColor = new OriColor(0x0C525F) }))
                BitmapUtils.Save(bmp, path, ImageFormat.Png);

            await Context.Channel.SendFileAsync(path, $"Draw `{text}`");
        }

        [Command("eventparsetest")]
        public async Task EventParseTestAsync([Remainder]string content)
        {
            if (!Checks.NotNull(content))
                await ReplyAsync("you numnut.");
            string result = OriFormat.ParseGreeting(content, new GuildEventContext(Context.Server, Context.Guild, Context.Guild.GetUser(Context.User.Id)));
            await Context.Channel.SendMessageAsync(result);
        }

        [RequireUserAccount]
        [Command("games")]
        [Summary("Returns a list of all visible **Games**.")]
        public async Task ShowLobbiesAsync([Summary("The page index for the list.")]int page = 1) // utilize a paginator.
            => await Context.Channel.SendMessageAsync(_gameManager.IsEmpty ? $"> **Looks like there's nothing here.**" : string.Join('\n', _gameManager.Games.Values.Select(x => x.ToString())));

        [RequireUserAccount]
        [Command("joingame"), Alias("jg")]
        [Summary("Join an open **Lobby**.")]
        public async Task JoinLobbyAsync([Summary("A string pointing to a specific **Game**.")]string id)
        {
            Game game = _gameManager[id];
            if (game == null)
                await Context.Channel.SendMessageAsync(_gameManager.ContainsUser(Context.User.Id) ?
                    "> **Wait a minute...**\n> You are already in a game." : $"**No luck.**\n> I couldn't find any games matching #**{id}**.");
            else
            {
                if (game.ContainsUser(Context.User.Id))
                    await Context.Channel.SendMessageAsync($"**???**\n> You are already in this game.");
                else
                {
                    await _gameManager.AddUserAsync(Context, id);
                    await Context.Channel.SendMessageAsync($"**Success!**\n> You have joined {game.Lobby.Name}. [{game.Receivers.First(x => x.Id == Context.Guild.Id).Mention}]");
                }
            }
        }

        [Command("whispertest")]
        public async Task WhisperTestAsync()
        {
            await WhisperAsync(".....hi.");
            await ReplyAsync("Psst... I sent you something");
        }

        [Command("roletoggletest")]
        public async Task RemoveRoleTestAsync()
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

        [Command("presettest")]
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

        [Command("creategame"), Alias("crg")]
        [Summary("Create a **Game**.")]
        [RequireUserAccount]
        public async Task StartLobbyAsync([Summary("The **GameMode** to play within the **Game**.")]GameMode mode)
        {
            if (_gameManager.ContainsUser(Context.Account.Id))
            {
                await Context.Channel.SendMessageAsync($"> **Wait a minute...**\n> You are already in a game.");
                return;
            }
            try
            {
                Game game = await _gameManager.CreateGameAsync(Context, new GameConfig(mode, $"{Context.User.Username}'s Lobby")).ConfigureAwait(false);
                await Context.Channel.SendMessageAsync($"**Success!**\n> {game.Lobby.Name} has been created. [{game.Receivers[0].Mention}]");
                await _gameManager.StartGameSessionAsync(game.Id);
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("colortest")]
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

        [Command("windowtest")]
        public async Task WindowTestAsync([Remainder]string message = null)
        {
            message = Checks.NotNull(message) ? message : "This is a generic message.";
            try
            {
                GameWindow window = new GameWindow(GameWindowProperties.Lobby);
                
                window.CurrentTab.GetElement("element.info").Update(OriFormat.Lobby("New Lobby", KeyBuilder.Generate(8), GamePrivacy.Local.ToString(), GameMode.Werewolf.ToString()));

                window.CurrentTab.AddToGroup("elements.console",
                    new Element(OriFormat.GameChat("Orikivo", "Message 1."), "message-0"));

                window.CurrentTab.AddToGroup("elements.console",
                    new Element(OriFormat.GameChat(Context.User.Username, message), "message-1"));

                window.CurrentTab.AddToGroup("elements.console",
                    new Element(OriFormat.GameChat(Context.User.Username, message), "message-2"));

                window.CurrentTab.AddToGroup("elements.console",
                    new Element(OriFormat.GameChat(Context.User.Username, message), "message-3"));

                window.CurrentTab.AddToGroup("elements.console",
                    new Element(OriFormat.GameChat(Context.User.Username, message), "message-4"));

                window.CurrentTab.AddToGroup("elements.console",
                    new Element(OriFormat.GameChat(Context.User.Username, message), "message-5"));

                window.CurrentTab.AddToGroup("elements.console",
                    new Element(OriFormat.GameChat(Context.User.Username, message), "message-6"));

                window.CurrentTab.AddToGroup("elements.console",
                    new Element(OriFormat.GameChat("Orikivo", "Message 8."), "message-7"));

                window.CurrentTab.AddToGroup("elements.console",
                    new Element(OriFormat.GameChat("Orikivo", "Message 9."), "message-8"));

                window.CurrentTab.GetElement("element.user_info").Update(OriFormat.UserCounter(1, 15));

                window.CurrentTab.AddToGroup("elements.triggers", new Element(new GameTrigger("start").ToString()));
                window.CurrentTab.AddToGroup("elements.users", new Element(new User(Context.User.Id, Context.User.Username, Context.Guild.Id, UserTag.Host | UserTag.Playing).ToString()));
                window.CurrentTab.AddToGroup("elements.users", new Element(new User(Context.Client.CurrentUser.Id, Context.Client.CurrentUser.Username, Context.Guild.Id, UserTag.Playing).ToString()));

                await Context.Channel.SendMessageAsync(window.Content);
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("triggertest")]
        public async Task TriggerTestAsync([Remainder]string message)
        {
            List<Player> players = new List<Player>();

            List<GameAttribute> playerAttributes = new List<GameAttribute>();
            Player player = new Player(Context.User.Id, Context.User.Username, playerAttributes);

            players.Add(player);

            List<GameAttribute> rootAttributes = new List<GameAttribute>();
            List<GameTrigger> rootTriggers = new List<GameTrigger>();
            GameData rootData = new GameData(players, rootAttributes, rootTriggers);

            List<GameAttribute> taskAttributes = new List<GameAttribute>();
            List<GameTrigger> taskTriggers = new List<GameTrigger>();

            List<GameArgValue> values = new List<GameArgValue>();

            
            List<GameCriterion> valueCriteria = new List<GameCriterion>();
            List<GameUpdatePacket> onValueParseSuccess = new List<GameUpdatePacket>();
            GameArgValue value = new GameArgValue("world", onValueParseSuccess, valueCriteria);
            values.Add(value);

            // trigger1
            List<GameCriterion> argCriteria = new List<GameCriterion>();
            List<GameUpdatePacket> onArgParseSuccess = new List<GameUpdatePacket>();
            GameObject defaultArgValue = new GameObject(GameObjectType.String, "world");

            GameArg arg = new GameArg("message", values, argCriteria, onArgParseSuccess);

            List<GameCriterion> triggerCriteria = new List<GameCriterion>();
            List<GameUpdatePacket> onTriggerParseSuccess = new List<GameUpdatePacket>();
            GameTrigger trigger = new GameTrigger("test", arg, triggerCriteria, onTriggerParseSuccess);

            // trigger2
            GameArg arg2 = new GameArg("messages", GameObjectType.String, isArray: true);
            GameTrigger trigger2 = new GameTrigger("testmany", arg2);

            //trigger3
            GameArg arg3 = new GameArg("user", GameObjectType.User);
            GameTrigger trigger3 = new GameTrigger("testuser", arg3);


            taskTriggers.Add(trigger);
            taskTriggers.Add(trigger2);
            taskTriggers.Add(trigger3);

            GameTaskData taskData = new GameTaskData(rootData.TaskId, taskAttributes, taskTriggers);
            taskData.Root = rootData;

            TaskTriggerContext context = new TaskTriggerContext(taskData, player, message);

            for(int i = 0; i < taskData.Triggers.Count; i++)
            {
                taskData.Triggers[i].TryParse(context, out GameTriggerResult result);
                if (result.Error == TriggerParseError.InvalidTrigger && i + 1 < taskData.Triggers.Count)
                    continue;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"IsSuccess: {result.IsSuccess}");
                sb.AppendLine($"Context: \"{message}\"");
                sb.AppendLine($"Error: {result.Error}");
                sb.AppendLine($"ErrorReason: {result.ErrorReason}");
                sb.AppendLine($"TriggerId: {result.TriggerId}");
                sb.AppendLine($"ArgId: {result.ArgId}");
                string objects = string.Join("\n\n", result.Objects.Select(x => $"GameObject =>\nType: {x.Type}\nValue: {x.Value}\nIsArray: {x.IsArray}"));
                string packets = string.Join("\n\n", result.Packets.Select(x =>
                {
                    StringBuilder gb = new StringBuilder();
                    gb.AppendLine("GameUpdatePacket =>");
                    gb.AppendLine(string.Join("\n\n", x.AttributePackets
                    .Select(y =>
                    {
                        StringBuilder ab = new StringBuilder();
                        ab.AppendLine($"AttributeUpdatePacket =>");
                        ab.AppendLine($"Id: {y.Id}");
                        ab.AppendLine($"Method: {y.Method}");
                        ab.AppendLine($"Amount: {y.Amount}");
                        return ab.ToString();
                    }).Concat(x.WindowPackets.Select(y =>
                    {
                        StringBuilder wb = new StringBuilder();
                        wb.AppendLine($"WindowUpdatePacket =>");
                        wb.AppendLine($"WindowId: {y.WindowId}");
                        wb.AppendLine($"Output: {y.Output}");
                        wb.AppendLine($"ToTabId: {y.ToTabId}");
                        wb.AppendLine(string.Join("\n\n", y.Packets.Select(z =>
                        {
                            StringBuilder tb = new StringBuilder();
                            tb.AppendLine("TabUpdatePacket =>");
                            tb.AppendLine($"TabId: {z.TabId}");
                            tb.AppendLine(string.Join("\n\n", z.Packets.Select(a =>
                            {
                                StringBuilder eb = new StringBuilder();
                                eb.AppendLine($"ElementUpdatePacket =>");
                                eb.AppendLine($"ElementId: {a.ElementId}");
                                eb.AppendLine($"GroupId: {a.GroupId}");
                                eb.AppendLine($"Index: {a.Index}");
                                eb.AppendLine($"ElementUpdateMethod: {a.Method}");
                                eb.AppendLine($"Element: {a.Element?.ToString()}");
                                return eb.ToString();
                            })));
                            return tb.ToString();
                        })));
                        return wb.ToString();
                    }))));
                    return gb.ToString();
                }));
                sb.AppendLine(objects);
                sb.AppendLine(packets);

                await Context.Channel.SendMessageAsync(sb.ToString());
                return;
            }
        }

        [Command("choosetest")]
        public async Task ChooseTestAsync(int times = 8, bool allowRepeats = true)
        {
            times = times > 8 ? 8 : times < 1 ? 1 : times; // force bounds

            List<int> values = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("I have picked the winners.");
            foreach (int value in OriRandom.ChooseMany(values, times, allowRepeats))
            {
                sb.AppendLine($"Selected: {value}");
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("shuffletest")]
        public async Task ShuffleTestAsync()
        {
            List<int> values = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Old: {{ {values.WriteValues()} }}");
            List<int> shuffledValues = OriRandom.Shuffle(values).ToList();
            sb.AppendLine($"OldEnsure: {{ {values.WriteValues()} }}");
            sb.AppendLine($"New: {{ {shuffledValues.WriteValues()} }}");
            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("dicetest")]
        public async Task DiceTestAsync()
        {
            Dice dice = Dice.Default;
            int result = OriRandom.Roll(dice);
            await Context.Channel.SendMessageAsync(result.ToString());
        }
    }
}
