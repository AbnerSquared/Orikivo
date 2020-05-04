using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Drawing;
using Orikivo.Drawing.Encoding;
using Orikivo.Drawing.Graphics2D;
using Orikivo.Drawing.Graphics3D;
using Orikivo.Gaming;
using Orikivo.Net;
using Orikivo.Text;
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
using Match = System.Text.RegularExpressions.Match;
using SysColor = System.Drawing.Color;
using Orikivo.Drawing.Animating;

namespace Orikivo
{
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

        private List<Stream> GetTextFrames(params string[] texts)
        {
            if (texts.Length == 0)
                throw new ArgumentNullException("At least one text must be written.");

            List<Stream> frames = new List<Stream>();

            FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            char[][][][] charMap = JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());

            GraphicsConfig config = new GraphicsConfig { CharMap = charMap };
            using (GraphicsWriter poxel = new GraphicsWriter(config))
            {
                poxel.DefaultOptions = new CanvasOptions { UseNonEmptyWidth = false, Padding = new Padding(2), BackgroundColor = new GammaColor(0x0C525F) };
                for (int i = 0; i < texts.Length; i++)
                {
                    MemoryStream stream = new MemoryStream();

                    using (Bitmap frame = poxel.DrawString(texts[i], font, GammaColor.GammaGreen))
                        frame.Save(stream, ImageFormat.Png);

                    frames.Add(stream);
                }
            }

            return frames;
        }

        [Command("debugdraw")]
        public async Task DebugDrawAsync()
        {
            if (Context.Account.Husk.Location.GetInnerType() == LocationType.Sector)
            {
                await SendPixelsAsync(Engine.DebugDraw(Context.Account.Husk.Location.GetLocation() as Sector, Context.Account.Husk), $"{Context.User.Id}_DEBUG_DRAW", 2);
            }
        }

        private async Task SendPixelsAsync(Grid<SysColor> pixels, string fileName, int imageScale = 1)
        {
            Bitmap image = GraphicsUtils.CreateRgbBitmap(pixels.Values);

            if (imageScale > 1)
                image = GraphicsUtils.Scale(image, imageScale, imageScale);

            using (image)
                await SendPngAsync($"../tmp/{fileName}.png", image);
        }

        [Command("drawcircle")]
        public async Task DrawCircleTestAsync(int imageSize, int originX, int originY, int radius, int imageScale = 1)
        {
            Canvas canvas = new Canvas(imageSize, imageSize, GammaPalette.GammaGreen[Gamma.Min]);
            canvas.DrawCircle(originX, originY, radius, GammaPalette.GammaGreen[Gamma.Standard]);

            await SendPixelsAsync(canvas.Pixels, $"{Context.User.Id}_CIRCLE", imageScale);

        }

        [Command("drawline")]
        public async Task DrawLineTestAsync(int imageSize, int ax, int ay, int bx, int by, int imageScale)
        {
            Canvas canvas = new Canvas(imageSize, imageSize, GammaPalette.GammaGreen[Gamma.Min]);
            canvas.DrawLine(ax, ay, bx, by, GammaPalette.GammaGreen[Gamma.Standard]);

            await SendPixelsAsync(canvas.Pixels, $"{Context.User.Id}_LINE", imageScale);
        }

        [Command("drawrect")]
        public async Task DrawRectTestAsync(int imageSize, int x, int y, int width, int height, int imageScale)
        {
            Canvas canvas = new Canvas(imageSize, imageSize, GammaPalette.GammaGreen[Gamma.Min]);
            canvas.DrawRectangle(x, y, width, height, GammaPalette.GammaGreen[Gamma.Standard]);

            await SendPixelsAsync(canvas.Pixels, $"{Context.User.Id}_RECT", imageScale);
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

        [Command("ping")]
        public async Task PingAsync()
            => await CoreProvider.PingAsync(Context.Channel, Context.Client);

        private Stream DrawFrame(GraphicsWriter graphics, string content, GammaColor color, CanvasOptions options)
        {
            MemoryStream stream = new MemoryStream();
            using (Bitmap frame = graphics.DrawString(content, color, options))
            {
                frame.Save(stream, ImageFormat.Png);
                // BitmapUtils.Save(frame, outputPath, ImageFormat.Png);
            }

            return stream;
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

        [Command("animtimeline")]
        public async Task AnimateTimelineAsync(double delay)
        {
            using (GraphicsWriter artist = new GraphicsWriter())
            {
                using (TimelineAnimator animator = new TimelineAnimator())
                {
                    animator.Ticks = 500;
                    animator.Viewport = new Size(128, 128);

                    Keyframe initial = new Keyframe(0, 1.0f, Vector2.Zero, 0.0f, Vector2.One);
                    Keyframe mid = new Keyframe(250, 1.0f, new Vector2(32, 32), 359.9f, new Vector2(1, 1));
                    Keyframe quarter = new Keyframe(375, 1.0f, new Vector2(16, 16), 180.0f, new Vector2(2, 2));
                    Keyframe final = new Keyframe(500, 1.0f, new Vector2(0, 0), 0.0f, new Vector2(1, 1));

                    List<Keyframe> keyframes = new List<Keyframe>();
                    keyframes.Add(mid);
                    keyframes.Add(quarter);
                    keyframes.Add(final);


                    TimelineLayer square = new TimelineLayer(
                        artist.DrawSolid(GammaPalette.GammaGreen[Gamma.Max], 32, 32),
                        keyframes,
                        0, 500);

                    TimelineLayer background = new TimelineLayer(
                        artist.DrawSolid(GammaPalette.GammaGreen[Gamma.Min], 128, 128),
                        new List<Keyframe> { initial },
                        0, 500);


                    animator.AddLayer(background);
                    animator.AddLayer(square);

                    MemoryStream animation = animator.Compile(TimeSpan.FromMilliseconds(delay));

                    await Context.Channel.SendGifAsync(animation, "../tmp/timeline_anim2.gif", quality: Quality.Bpp8);
                }
            }
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("balance"), Alias("money", "bal")]
        public async Task GetMoneyAsync()
        {
            StringBuilder values = new StringBuilder();

            values.AppendLine($"**Balance**: 💸 **{Context.Account.Balance.ToString("##,0.###")}**");
            values.AppendLine($"**Tokens**: 🏷️ **{Context.Account.TokenBalance.ToString("##,0.###")}**");
            values.AppendLine($"**Debt**: 📃 **{Context.Account.Debt.ToString("##,0.###")}**");

            await Context.Channel.SendMessageAsync(values.ToString());
        }

        [Command("border")]
        public async Task DrawBorderAsync()
        {
            using (Bitmap tmp = new Bitmap(32, 32))
                using (GraphicsWriter g = new GraphicsWriter())
                using (Bitmap border = g.DrawBorder(tmp, GammaColor.GammaGreen, 2))
                    await SendPngAsync("../tmp/border.png", border);
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("cardmask")]
        public async Task DrawCardMaskAsync()
        {
            using (GraphicsService g = new GraphicsService())
            {
                CardDetails d = new CardDetails(Context.Account, Context.User);

                using (Bitmap card = g.DrawCard(d, GammaPalette.GammaGreen))
                {
                    using (GraphicsWriter p = new GraphicsWriter())
                    {
                        Grid<float> mask = p.GetOpacityMask(card);

                        using (Bitmap gradient = p.DrawGradient(GammaPalette.GammaGreen, card.Width, card.Height, 0.0f, GradientColorHandling.Snap))
                            using (Bitmap result = p.SetOpacityMask(gradient, mask))
                                await SendPngAsync($"../tmp/{Context.User.Id}_gradient_card_mask.png", result);
                    }
                }
            }
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("mask")]
        public async Task DrawMaskAsync()
        {
            using (GraphicsService g = new GraphicsService())
            {
                CardDetails d = new CardDetails(Context.Account, Context.User);

                using (Bitmap card = g.DrawCard(d, GammaPalette.GammaGreen))
                {
                    using (GraphicsWriter p = new GraphicsWriter())
                    {
                        Grid<float> mask = p.GetOpacityMask(card);

                        Grid<SysColor> pixels = new Grid<SysColor>(card.Size, new GammaColor(0, 0, 0, 255));

                        pixels.SetEachValue((int x, int y) =>
                            GammaColor.Merge(new GammaColor(0, 0, 0, 255),
                                new GammaColor(255, 255, 255, 255),
                                mask.GetValue(x, y)));

                        using (Bitmap masked = GraphicsUtils.CreateRgbBitmap(pixels.Values))
                            await SendPngAsync($"../tmp/{Context.User.Id}_card_mask.png", masked);
                    }
                }
            }
        }

        [Command("gradient")]
        public async Task DrawGradientAsync()
        {
            GradientLayer gradient = new GradientLayer
            { Width = 64, Height = 32, Angle = AngleF.Right,
                ColorHandling  = GradientColorHandling.Blend };

            gradient.Markers[0.0f] = GammaPalette.NeonRed[Gamma.Min];
            gradient.Markers[0.25f] = GammaPalette.GammaGreen[Gamma.Standard];
            gradient.Markers[0.5f] = GammaPalette.NeonRed[Gamma.Bright];
            gradient.Markers[0.75f] = GammaPalette.Alconia[Gamma.Brighter];
            gradient.Markers[1.0f] = GammaPalette.Glass[Gamma.Max];

            await SendPngAsync("../tmp/gradient.png", gradient.Build());
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("card")]
        public async Task GetCardAsync(SocketUser user = null)
        {
            user ??= Context.User;
            Context.Container.TryGetUser(user.Id, out User account);

            try
            {
                using (GraphicsService graphics = new GraphicsService())
                {
                    CardDetails d = new CardDetails(account, user);
                    Bitmap card = graphics.DrawCard(d, GammaPalette.Glass, false, true);

                    await Context.Channel.SendImageAsync(card, $"../tmp/{Context.User.Id}_card.png");
                }
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("wait")]
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

        [Command("collect")]
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

        //[Command("charfill")]
        public async Task CharFillAsync(char fill, char empty, int width, float percent)
        {
            CharFill filler = new CharFill { FillChar = fill, EmptyChar = empty, Width = width, Percent = percent };
            int filled = (int)MathF.Floor(RangeF.Convert(0.0f, 1.0f, 0, width, percent));

            StringBuilder sb = new StringBuilder();

            sb.Append(filler.FillChar, filled);
            sb.Append(filler.EmptyChar, filler.Width - filled);

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        //[Command("wolfnode")]
        public async Task CreateWolfNodeAsync(string sessionName, string sessionId, string privacy, string gameMode, string password = null)
        {
            WerewolfInfoNode node = new WerewolfInfoNode { SessionName = sessionName,
            SessionId = sessionId, Privacy = privacy, GameMode = gameMode, Password = password };

            await Context.Channel.SendMessageAsync(node.ToString());
        }

        //[Command("consolenode")]
        public async Task CreateConsoleNodeAsync(string sessionName, string sessionId, string privacy, string gameMode, string message, string author = null)
        {
            WerewolfInfoNode node = new WerewolfInfoNode { SessionName = sessionName,
            SessionId = sessionId, Privacy = privacy, GameMode = gameMode,
            Messages = new List<MessageNode> {
                new MessageNode { Author = "Orikivo", Message = "vroom" },
                new MessageNode { Message = message, Author = author } }
            };

            await Context.Channel.SendMessageAsync(node.ToString());
        }

        [Command("drawb")]
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

        [Command("cgol")]
        [Summary("Simulates **Conway's Game of Life** for a specified width and height.")]
        public async Task RunLifeAsync(int width, int height, int duration, int delay = 100)
        {
            string path = $"../tmp/{Context.User.Id}_cgol.gif";
            GammaPalette colors = GammaPalette.Glass;
            Grid<ConwayCell> pattern = ConwayRenderer.GetRandomPattern(width, height);
            ConwayRenderer simulator = new ConwayRenderer(colors[Gamma.Max], colors[Gamma.Min], null, pattern);
            List<Grid<SysColor>> rawFrames = simulator.Run(duration);

            MemoryStream gifStream = new MemoryStream();
            using (GifEncoder encoder = new GifEncoder(gifStream))
            {
                List<Bitmap> frames = rawFrames.Select(f => GraphicsUtils.CreateRgbBitmap(f.Values)).ToList();
                encoder.FrameLength = TimeSpan.FromMilliseconds(delay);

                foreach (Bitmap frame in frames)
                    using (frame)
                        encoder.EncodeFrame(frame);
            }

            gifStream.Position = 0;
            Image gifResult = Image.FromStream(gifStream);
            gifResult.Save(path, ImageFormat.Gif);
            gifStream.Dispose();

            await Context.Channel.SendFileAsync(path);
        }

        private Rasterizer GetRasterizer(RasterizerType type)
        {
            return type switch
            {
                RasterizerType.Wireframe => new WireframeRasterizer(),
                RasterizerType.Solid => new SolidRasterizer(),
                _ => new SolidRasterizer()
            };
        }

        [Command("drawcube")]
        [Summary("Draws a cube with the specified parameters.")]
        public async Task DrawCubeWireAsync(float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f, float rotX = 0.0f, float rotY = 0.0f, float rotZ = 0.0f,
            float fov = 90.0f, int width = 128, int height = 128, RasterizerType rasterizer = RasterizerType.Wireframe)
        {
            string path = $"../tmp/STATIC_MESH_{KeyBuilder.Generate(8)}.png";

            MeshRenderer renderer = new MeshRenderer(width, height, fov, 0.1f, 1000.0f,
                GammaPalette.GammaGreen, GetRasterizer(rasterizer));

            Grid<SysColor> frame = renderer.Render(Mesh.Cube,
                new Vector3(posX, posY, posZ), 
                new Vector3(rotX, rotY, rotZ));

            await SendPngAsync(path, frame);
        }

        [Command("animcube")]
        [Summary("Animates a cube mesh in a 3D environment with the specified parameters.")]
        public async Task AnimCubeAsync(
            [Summary("The amount of frames the animation is drawn for.")]long ticks,
            [Summary("The initial X offset of the mesh.")]float offsetX = 0.0f,
            [Summary("The initial Y offset of the mesh.")]float offsetY = 0.0f,
            [Summary("The initial Z offset of the mesh.")]float offsetZ = 0.0f,
            [Summary("The X rotation velocity of the mesh.")]float rotX = 0.0f,
            [Summary("The Y rotation velocity of the mesh.")]float rotY = 0.0f,
            [Summary("The Z rotation velocity of the mesh.")]float rotZ = 0.0f,
            [Summary("The X velocity of the mesh.")]float velocityX = 0.0f,
            [Summary("The Y velocity of the mesh.")]float velocityY = 0.0f,
            [Summary("The Z velocity of the mesh.")]float velocityZ = 0.0f,
            [Summary("The amount of frames per second the animation is drawn at.")]int fps = 60,
            [Summary("The field of view for the viewport (in degrees).")]float fov = 90.0f,
            [Summary("The width of the viewport.")]int width = 128,
            [Summary("The height of the viewport.")]int height = 128,
            [Summary("The rasterizer that is used when drawing the mesh.")] RasterizerType rasterizer = RasterizerType.Wireframe)
        {
            try
            {
                string path = $"../tmp/MESH_{KeyBuilder.Generate(8)}.gif";
                GammaPalette colors = GammaPalette.GammaGreen;

                MeshRenderer renderer = new MeshRenderer(width, height, fov, 0.1f, 1000.0f,
                    GammaPalette.GammaGreen, GetRasterizer(rasterizer));

                List<Grid<SysColor>> frames = renderer.Animate(ticks, new Model(Mesh.Cube),
                    new Vector3(offsetX, offsetY, offsetZ),
                    new Vector3(rotX, rotY, rotZ),
                    new Vector3(velocityX, velocityY, velocityZ));
                
                await SendGifAsync(path, frames, 1000 / fps);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        }

        private async Task SendGifAsync(string path, List<Grid<SysColor>> rawFrames, int delay = 100, int? loop = null)
        {
            try
            {
                MemoryStream gifStream = new MemoryStream();
                using (GifEncoder encoder = new GifEncoder(gifStream))
                {
                    List<Bitmap> frames = rawFrames.Select(f => GraphicsUtils.CreateRgbBitmap(f.Values)).ToList();
                    encoder.FrameLength = TimeSpan.FromMilliseconds(delay);

                    foreach (Bitmap frame in frames)
                        using (frame)
                            encoder.EncodeFrame(frame);
                }

                gifStream.Position = 0;
                Image gifResult = Image.FromStream(gifStream);
                gifResult.Save(path, ImageFormat.Gif);
                gifStream.Dispose();

                await Context.Channel.SendFileAsync(path);
            }
            catch(Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
            finally
            {
                //File.Delete(path);
            }
        }

        [Command("cgold")]
        [Summary("Simulates **Conway's Game of Life** with a color decay system.")]
        public async Task RunLifeDecayAsync(int duration, ulong decayLength, int delay = 100)
        {// int width, int height, 
            string path = $"../tmp/{Context.User.Id}_cgol.gif";
            GammaPalette colors = GammaPalette.Alconia;
            Grid<ConwayCell> pattern = Pattern;
            ConwayRenderer simulator = new ConwayRenderer(GammaPalette.GammaGreen[Gamma.Standard], colors[Gamma.Min], decayLength, pattern);
            simulator.ActiveColor = GammaPalette.GammaGreen[Gamma.Max];
            List<Grid<SysColor>> rawFrames = simulator.Run(duration);

            await SendGifAsync(path, rawFrames, delay);
        }


        [Command("timecycle")]
        public async Task CycleTimeAsync(int framesPerHour = 1, int delay = 150, int? loop = null)
        {
            string path = $"../tmp/{Context.User.Id}_time.gif";
            FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            char[][][][] charMap = JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());
            GraphicsConfig config = new GraphicsConfig { CharMap = charMap };
            List<Stream> frames = new List<Stream>();
            CanvasOptions options = new CanvasOptions { UseNonEmptyWidth = true, Padding = new Padding(2), Width = 47 };
            float t = 0.00f;
            using (GraphicsWriter poxel = new GraphicsWriter(config))
            {
                poxel.SetFont(font);
                
                for (float h = 0; h < 24 * framesPerHour; h++)
                {
                    Console.WriteLine($"HOUR:{t}");
                    GammaPalette colors = TimeCycle.FromHour(t);
                    poxel.Palette = colors;
                    options.BackgroundColor = colors[Gamma.Min];

                    frames.Add(DrawFrame(poxel, t.ToString("00.00H"), colors[Gamma.Max], options));

                    t += (1.00f / framesPerHour);
                }
            }

            MemoryStream gifStream = new MemoryStream();
            using (GifEncoder encoder = new GifEncoder(gifStream, repeatCount: loop))
            {
                encoder.FrameLength = TimeSpan.FromMilliseconds(delay);

                foreach (Stream frame in frames)
                    using (frame)
                        encoder.EncodeFrame(Image.FromStream(frame));
            }

            gifStream.Position = 0;
            Image gifResult = Image.FromStream(gifStream);
            gifResult.Save(path, ImageFormat.Gif);
            gifStream.Dispose();

            await Context.Channel.SendFileAsync(path);
        }

        [Command("timef")]
        public async Task GetTimeAsync(float hour)
        {
            if (hour > 23.00f || hour < 0.00f)
            {
                await Context.Channel.SendMessageAsync("hour out of range");
                return;
            }

            try
            {
                string path = $"../tmp/{Context.User.Id}_time.png";
                FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
                // new OutlineProperties(1, new OriColor(0x44B29B)): Too taxing on performance as of now
                char[][][][] charMap = JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());
                GammaPalette colors = TimeCycle.FromHour(hour);
                GraphicsConfig config = new GraphicsConfig { CharMap = charMap, Palette = colors };
                using (GraphicsWriter poxel = new GraphicsWriter(config))
                using (Bitmap bmp = poxel.DrawString(hour.ToString("00.00H").ToUpper(), font,
                    new CanvasOptions { UseNonEmptyWidth = true, Padding = new Padding(2), BackgroundColor = colors[Gamma.Min] }))
                    BitmapHandler.Save(bmp, path, ImageFormat.Png);

                await Context.Channel.SendFileAsync(path);
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        public async Task SendPngAsync(string path, Grid<SysColor> grid)
        {
            await SendPngAsync(path, GraphicsUtils.CreateRgbBitmap(grid.Values));
        }

        public async Task SendPngAsync(string path, Bitmap bmp)
        {
            BitmapHandler.Save(bmp, path, ImageFormat.Png);
            await Context.Channel.SendFileAsync(path);
        }

        

        [Command("time")]
        [Summary("Shows the current time of day.")]
        public async Task GetTimeAsync()
        {
            try
            {
                string path = $"../tmp/{Context.User.Id}_time.png";
                GammaPalette palette = TimeCycle.FromUtcNow();

                using (GraphicsService graphics = new GraphicsService())
                {
                    Bitmap bmp = graphics.DrawString(DateTime.UtcNow.ToString("hh:mm tt").ToUpper(), Gamma.Max, palette);
                    await Context.Channel.SendImageAsync(bmp, path);
                }
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("animscroll")]
        [Summary("Creates a GIFASCII animation using the AsciiEngine.")]
        public async Task DrawRenderAsync(
            [Summary("The value to scroll across the grid.")]string text = "NEW TEXT",
            [Summary("The width of the grid.")]int width = 16,
            [Summary("The height of the grid.")]int height = 4,
            [Summary("The X velocity component of this object.")]float xVelocity = 1.0f,
            [Summary("The Y velocity component of this object.")]float yVelocity = 0.0f,
            [Summary("The amount of frames to render.")]int frameCount = 10,
            [Summary("The amount of time to increment by.")]float step = 1.00f,
            [Summary("The amount of time each frame is shown (in milliseconds).")]int delay = 150)
        {
            int minWidth = text.Split('\n').OrderByDescending(x => x.Length).First().Length;
            int minHeight = text.Split('\n').Length;
            using (var engine = new AsciiEngine(width < minWidth ? minWidth : width, height < minHeight ? minHeight : height))
            {
                //engine.CurrentGrid.CreateAndAddObject("IGNORE", '\n', 0, 0, 0, GridCollideMethod.Ignore, new AsciiVector(1, 0, 0, 0));
                //engine.CurrentGrid.CreateAndAddObject("DVD", '\n', 0, 1, 0, GridCollideMethod.Reflect, new AsciiVector(1, 0, 0, 0));
                engine.CurrentGrid.CreateAndAddObject(text, '\n', 0, 0, 0, GridCollideMethod.Scroll, new AsciiVector(xVelocity, yVelocity, 0, 0));
                //engine.CurrentGrid.CreateAndAddObject("STOP", '\n', 0, 3, 0, GridCollideMethod.Stop, new AsciiVector(1, 0, 0, 0));
                string[] frames = engine.GetFrames(0, frameCount, step);
                await DrawAnimAsync(delay, frames);
            }
        }

        /// <summary>
        /// Creates a GIFASCII animation from a given .txt file with an optional specified delay.
        /// </summary>
        /// <param name="delay">The length of delay per frame (in milliseconds).</param>
        [Command("animateasciif")]
        [Summary("Creates a GIFASCII animation from a given .txt file with an optional specified delay (in milliseconds).")]
        [RequireAttachment(FileFormat.Text, "frames")]
        public async Task DrawAnimAsync([Summary("The length of delay per frame (in milliseconds).")]int delay = 150, int? loop = null)
        {
            Attachment content = Context.Message.Attachments.Where(x => EnumUtils.GetUrlType(x.Filename) == UrlType.Text).First();

            using (OriWebClient client = new OriWebClient())
            {
                OriWebResult urlResult = await client.RequestAsync(content.Url);

                string[] urlFrames = ParseFrames(urlResult.RawContent);

                if (urlFrames.Length == 0)
                    throw new Exception("There weren't any proper frames specified.");

                await DrawAnimAsync(delay, loop, urlFrames);
            }
        }

        private string[] ParseFrames(string rawContent)
        {
            List<string> frames = new List<string>();
            Regex regex = new Regex("([\"\'])(?:(?=(\\\\?))\\2[\\s\\S])*?\\1");
            MatchCollection matches = regex.Matches(rawContent);
            foreach(Match match in matches)
            {
                //Console.WriteLine($"Match!:\n{string.Join(", ", match.Value.Select(x => $"\'{x}\'"))}");
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

        [Command("drawanimdr")]
        public async Task DrawAnimAsync(int millisecondDelay, int? loop, params string[] strings)
        {
            try
            {
                string gifPath = $"../tmp/{Context.User.Id}_anim.gif";
                FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
                MemoryStream gifStream = new MemoryStream();
                using (GifEncoder encoder = new GifEncoder(gifStream, repeatCount: loop))
                {
                    encoder.FrameLength = TimeSpan.FromMilliseconds(millisecondDelay);

                    foreach (Stream frame in GetTextFrames(strings))
                        using (frame)
                            encoder.EncodeFrame(Image.FromStream(frame));
                }

                gifStream.Position = 0;
                Image gifResult = Image.FromStream(gifStream);
                gifResult.Save(gifPath, ImageFormat.Gif);
                gifStream.Dispose();

                await Context.Channel.SendFileAsync(gifPath);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
            //await Context.Channel.SendFileAsync(gifStream, "gif_test.gif");
        }

        [Command("drawanimd")]
        public async Task DrawAnimAsync(int millisecondDelay, params string[] strings)
        {
            string gifPath = $"../tmp/{Context.User.Id}_anim.gif";
            FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            MemoryStream gifStream = new MemoryStream();
            using (GifEncoder encoder = new GifEncoder(gifStream))
            {
                encoder.FrameLength = TimeSpan.FromMilliseconds(millisecondDelay);

                foreach (Stream frame in GetTextFrames(strings))
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
            EmbedBuilder eb = new EmbedBuilder();
            
            string path = $"../tmp/{Context.User.Id}_text.png";
            FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            // new OutlineProperties(1, new OriColor(0x44B29B)): Too taxing on performance as of now
            char[][][][] charMap = JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());
            GraphicsConfig config = new GraphicsConfig { CharMap = charMap };
            using (GraphicsWriter poxel = new GraphicsWriter(config))
            using (Bitmap bmp = poxel.DrawString(text, font, GammaColor.GammaGreen, new CanvasOptions { UseNonEmptyWidth = true, Padding = new Padding(2), BackgroundColor = new GammaColor(0x0C525F) }))
                    BitmapHandler.Save(bmp, path, ImageFormat.Png);

            await Context.Channel.SendFileAsync(path);
        }

        [Command("drawm")]
        [Summary("Draws a pixelated string as monospaced text.")]
        public async Task DrawMonoTextAsync([Remainder]string text)
        {
            string path = $"../tmp/{Context.User.Id}_text.png";
            FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/monori.json");
            // new OutlineProperties(1, new OriColor(0x44B29B)): Too taxing on performance as of now
            char[][][][] charMap = JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());

            GraphicsConfig config = new GraphicsConfig { CharMap = charMap };
            using (GraphicsWriter poxel = new GraphicsWriter(config))
            using (Bitmap bmp = poxel.DrawString(text, font, GammaColor.GammaGreen, new CanvasOptions { UseNonEmptyWidth = false, Padding = new Padding(2), BackgroundColor = new GammaColor(0x0C525F) }))
                    BitmapHandler.Save(bmp, path, ImageFormat.Png);

            await Context.Channel.SendFileAsync(path);
        }

        /*
        [RequireUser]
        [Command("games")]
        [Summary("Returns a list of all visible **Games**.")]
        public async Task ShowLobbiesAsync([Summary("The page index for the list.")]int page = 1) // utilize a paginator.
            => await Context.Channel.SendMessageAsync(_gameManager.IsEmpty ? $"> **Looks like there's nothing here.**" : string.Join('\n', _gameManager.Games.Values.Select(x => x.ToString())));
        */

        /*
        [RequireUser]
        [Command("joingame"), Alias("jg")]
        [Summary("Join an open **Lobby**.")]
        public async Task JoinLobbyAsync([Summary("A string pointing to a specific **Game**.")]string id)
        {
            Game game = _gameManager[id];
            if (game == null)
                await Context.Channel.SendMessageAsync(_gameManager.ContainsUser(Context.User.Id) ?
                    "**Wait a minute...**\n> You are already in a game." : $"**No luck.**\n> I couldn't find any games matching #**{id}**.");
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
        */

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

        /*
        [Command("creategame"), Alias("crg")]
        [Summary("Create a **Game**.")]
        [RequireUser]
        public async Task StartLobbyAsync([Summary("The **GameMode** to play within the **Game**.")]GameMode mode)
        {
            if (_gameManager.ContainsUser(Context.Account.Id))
            {
                await Context.Channel.SendMessageAsync($"**Wait a minute...**\n> You are already in a game.");
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
        }*/

        [Command("randomchoose")]
        public async Task ChooseTestAsync(int times = 8, bool allowRepeats = true)
        {
            times = times > 8 ? 8 : times < 1 ? 1 : times; // force bounds

            List<int> values = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("I have picked the winners.");
            foreach (int value in Randomizer.ChooseMany(values, times, allowRepeats))
            {
                sb.AppendLine($"Selected: {value}");
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("randomshuffle")]
        public async Task ShuffleTestAsync()
        {
            List<int> values = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Old: {{ {string.Join(", ", values)} }}");
            List<int> shuffledValues = Randomizer.Shuffle(values).ToList();
            sb.AppendLine($"OldEnsure: {{ {string.Join(", ", values)} }}");
            sb.AppendLine($"New: {{ {string.Join(", ", shuffledValues)} }}");
            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("dice")]
        public async Task DiceTestAsync()
        {
            Dice dice = Dice.Default;
            int result = Randomizer.Roll(dice);
            await Context.Channel.SendMessageAsync(result.ToString());
        }
    }
}
