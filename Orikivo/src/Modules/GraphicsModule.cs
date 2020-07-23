using Discord;
using Discord.Commands;
using Orikivo.Drawing;
using Orikivo.Drawing.Encoding;
using Orikivo.Drawing.Graphics2D;
using Orikivo.Net;
using Orikivo.Text;
using Orikivo.Desync;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Image = System.Drawing.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Match = System.Text.RegularExpressions.Match;
using Color = System.Drawing.Color;
using Orikivo.Drawing.Animating;
using Orikivo.Drawing.Graphics3D;
using Discord.WebSocket;
using Orikivo.Framework;

namespace Orikivo.Modules
{
    [Name("Graphics")]
    [Summary("Provides methods that utilize the graphics engine.")]
    public class GraphicsModule : OriModuleBase<DesyncContext>
    {
        private async Task SendImageAsync(Grid<Color> pixels, string name, int scale = 1)
        {
            await Context.Channel.SendImageAsync(GraphicsService.GetBitmap(pixels, scale), $"../tmp/{name}.png");
        }
        private async Task SendPixelsAsync(Grid<Color> pixels, string fileName, int imageScale = 1)
        {
            Bitmap image = ImageEditor.CreateRgbBitmap(pixels.Values);

            if (imageScale > 1)
                image = ImageHelper.Scale(image, imageScale, imageScale);

            using (image)
                await Context.Channel.SendImageAsync(image, $"../tmp/{fileName}.png");
        }

        private List<Stream> GetTextFrames(params string[] texts)
        {
            if (texts.Length == 0)
                throw new ArgumentNullException("At least one text must be written.");

            var frames = new List<Stream>();

            FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            char[][][][] charMap = JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());

            var config = TextFactoryConfig.Default;
            config.CharMap = charMap;

            using (var factory = new TextFactory(config))
            {
                var properties = new ImageProperties
                {
                    TrimEmptyPixels = false,
                    Padding = new Padding(2),
                    Matte = new ImmutableColor(0x0C525F)
                };

                for (int i = 0; i < texts.Length; i++)
                {
                    var stream = new MemoryStream();

                    using (Bitmap frame = factory.DrawText(texts[i], font, ImmutableColor.GammaGreen, properties))
                        frame.Save(stream, ImageFormat.Png);

                    frames.Add(stream);
                }
            }

            return frames;
        }

        [Command("smearpalette")]
        public async Task SmearAsync(PaletteType a, PaletteType b)
        {
            GammaPalette result = GammaPalette.Smear(GraphicsService.GetPalette(a), GraphicsService.GetPalette(b));

            await Context.Channel.SendImageAsync(ImageEditor.CreateGradient(result, 128, 64, AngleF.Right),
                "../tmp/smear.png");
        }

        [Command("animatetimeline")]
        public async Task AnimateTimelineAsync(double delay)
        {
            using var animator = new TimelineAnimator();

            animator.Ticks = 500;
            animator.Viewport = new Size(128, 128);

            var initial = new Keyframe(0, 1.0f, Vector2.Zero, 0.0f, Vector2.One);
            var mid = new Keyframe(250, 1.0f, new Vector2(32, 32), 359.9f, new Vector2(1, 1));
            var quarter = new Keyframe(375, 1.0f, new Vector2(16, 16), 180.0f, new Vector2(2, 2));
            var final = new Keyframe(500, 1.0f, new Vector2(0, 0), 0.0f, new Vector2(1, 1));

            var square = new TimelineLayer(ImageEditor.CreateSolid(GammaPalette.GammaGreen[Gamma.Max], 32, 32), 0, 500, mid, quarter, final);

            var background = new TimelineLayer(ImageEditor.CreateSolid(GammaPalette.GammaGreen[Gamma.Min], 128, 128), 0, 500, initial);

            animator.AddLayer(background);
            animator.AddLayer(square);

            MemoryStream animation = animator.Compile(TimeSpan.FromMilliseconds(delay));

            await Context.Channel.SendGifAsync(animation, "../tmp/timeline_anim2.gif", quality: Quality.Bpp8);
        }

        [Command("drawgradient")]
        public async Task DrawGradientAsync()
        {
            var gradient = new GradientLayer
            {
                Width = 64,
                Height = 32,
                Angle = AngleF.Right,
                ColorHandling = GradientColorHandling.Blend
            };

            gradient.Markers[0.0f] = GammaPalette.NeonRed[Gamma.Min];
            gradient.Markers[0.25f] = GammaPalette.GammaGreen[Gamma.Standard];
            gradient.Markers[0.5f] = GammaPalette.NeonRed[Gamma.Bright];
            gradient.Markers[0.75f] = GammaPalette.Alconia[Gamma.Brighter];
            gradient.Markers[1.0f] = GammaPalette.Glass[Gamma.Max];

            await Context.Channel.SendImageAsync(gradient.Build(), "../tmp/gradient.png");
        }

        [Command("drawcircle")]
        public async Task DrawCircleAsync(int imageSize, int originX, int originY, int radius, int imageScale = 1)
        {
            var canvas = new Canvas(imageSize, imageSize, GammaPalette.GammaGreen[Gamma.Min]);
            canvas.DrawCircle(originX, originY, radius, GammaPalette.GammaGreen[Gamma.Standard]);

            await SendImageAsync(canvas.Pixels, $"{Context.User.Id}_CIRCLE", imageScale);
        }

        [Command("drawline")]
        public async Task DrawLineAsync(int imageSize, int ax, int ay, int bx, int by, int imageScale)
        {
            var canvas = new Canvas(imageSize, imageSize, GammaPalette.GammaGreen[Gamma.Min]);
            canvas.DrawLine(ax, ay, bx, by, GammaPalette.GammaGreen[Gamma.Standard]);

            await SendPixelsAsync(canvas.Pixels, $"{Context.User.Id}_LINE", imageScale);
        }

        [Command("drawrectangle")]
        public async Task DrawRectangleAsync(int imageSize, int x, int y, int width, int height, int imageScale)
        {
            Canvas canvas = new Canvas(imageSize, imageSize, GammaPalette.GammaGreen[Gamma.Min]);
            canvas.DrawRectangle(x, y, width, height, GammaPalette.GammaGreen[Gamma.Standard]);

            await SendPixelsAsync(canvas.Pixels, $"{Context.User.Id}_RECT", imageScale);
        }

        [Command("drawsector")]
        public async Task DebugDrawAsync()
        {
            if (Context.Account.Husk.Location.GetInnerType() == LocationType.Sector)
            {
                await SendPixelsAsync(Engine.DebugDraw(Context.Account.Husk.Location.GetLocation() as Sector, Context.Account.Husk), $"{Context.User.Id}_DEBUG_DRAW", 2);
            }
        }

        [Command("drawstring")]
        [Summary("Draws a string.")]
        public async Task DrawStringAsync([Remainder]string text)
        {
            EmbedBuilder eb = new EmbedBuilder();

            string path = $"../tmp/{Context.User.Id}_text.png";
            FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            // new OutlineProperties(1, new OriColor(0x44B29B)): Too taxing on performance as of now
            char[][][][] charMap = JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());
            var config = TextFactoryConfig.Default;
            config.CharMap = charMap;
            var properties = new ImageProperties { TrimEmptyPixels = true, Padding = new Padding(2), Matte = new ImmutableColor(0x0C525F) };

            using (var factory = new TextFactory(config))
                using (Bitmap bmp = factory.DrawText(text, font, ImmutableColor.GammaGreen, properties))
                    ImageHelper.Save(bmp, path, ImageFormat.Png);

            await Context.Channel.SendFileAsync(path);
        }

        [Command("drawmonospacedstring")]
        [Summary("Draws a string as monospaced text.")]
        public async Task DrawMonoTextAsync([Remainder]string text)
        {
            string path = $"../tmp/{Context.User.Id}_text.png";
            FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/monori.json");
            // new OutlineProperties(1, new OriColor(0x44B29B)): Too taxing on performance as of now
            char[][][][] charMap = JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());
            var config = TextFactoryConfig.Default;
            config.CharMap = charMap;
            var properties = new ImageProperties { TrimEmptyPixels = true, Padding = new Padding(2), Matte = new ImmutableColor(0x0C525F) };

            using (var factory = new TextFactory(config))
            using (Bitmap bmp = factory.DrawText(text, font, ImmutableColor.GammaGreen, properties))
                ImageHelper.Save(bmp, path, ImageFormat.Png);

            await Context.Channel.SendFileAsync(path);
        }

        [Command("engineanimateascii")]
        [Summary("Creates a GIFASCII animation using the AsciiEngine.")]
        public async Task DrawRenderAsync(
            [Summary("The value to scroll across the grid.")]string text = "NEW TEXT",
            [Summary("The collision method that is used for this object")]GridCollideMethod collideMethod = GridCollideMethod.Scroll,
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
                engine.CurrentGrid.CreateAndAddObject(text, '\n', 0, 0, 0, collideMethod, new AsciiVector(xVelocity, yVelocity, 0, 0));
                string[] frames = engine.GetFrames(0, frameCount, step);
                await DrawAnimAsync(delay, frames);
            }
        }

        /// <summary>
        /// Creates a GIFASCII animation from a provided attachment.
        /// </summary>
        [Command("animateasciifrom")]
        [Summary("Creates a GIFASCII animation from a provided attachment.")]
        [RequireAttachment(ExtensionType.Text, "frames")]
        public async Task DrawAnimAsync([Summary("The length of delay per frame (in milliseconds).")]int delay = 150, int? loop = null)
        {
            Attachment content = Context.Message.Attachments.Where(x => EnumUtils.GetUrlExtension(x.Filename) == ExtensionType.Text).First();

            using (var client = new OriWebClient())
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
            foreach (Match match in matches)
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

        [Command("animateascii")]
        public async Task DrawAnimAsync(params string[] strings)
            => await DrawAnimAsync(150, strings);

        [Command("animateasciifor")]

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

        [Command("animateasciidelay")]
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

        [Command("conway")]
        [Summary("Simulates **Conway's Game of Life** for a specified width and height.")]
        public async Task RunLifeAsync(int width, int height, int duration, int delay = 100)
        {
            string path = $"../tmp/{Context.User.Id}_cgol.gif";
            GammaPalette colors = GammaPalette.Glass;
            Grid<ConwayCell> pattern = ConwayRenderer.GetRandomPattern(width, height);
            ConwayRenderer simulator = new ConwayRenderer(colors[Gamma.Max], colors[Gamma.Min], null, pattern);
            List<Grid<Color>> rawFrames = simulator.Run(duration);

            MemoryStream gifStream = new MemoryStream();
            using (GifEncoder encoder = new GifEncoder(gifStream))
            {
                List<Bitmap> frames = rawFrames.Select(f => ImageEditor.CreateRgbBitmap(f.Values)).ToList();
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

            Grid<Color> frame = renderer.Render(Mesh.Cube,
                new Vector3(posX, posY, posZ),
                new Vector3(rotX, rotY, rotZ));

            await Context.Channel.SendImageAsync(ImageEditor.CreateRgbBitmap(frame.Values), path);
        }

        [Command("animatecube")]
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

                List<Grid<Color>> frames = renderer.Animate(ticks, new Model(Mesh.Cube),
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

        private async Task SendGifAsync(string path, List<Grid<Color>> rawFrames, int delay = 100, int? loop = null)
        {
            try
            {
                MemoryStream gifStream = new MemoryStream();
                using (GifEncoder encoder = new GifEncoder(gifStream))
                {
                    List<Bitmap> frames = rawFrames.Select(f => ImageEditor.CreateRgbBitmap(f.Values)).ToList();
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
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
            finally
            {
                //File.Delete(path);
            }
        }

        [Command("conwaydecay"), Alias("cgold")]
        [Summary("Simulates **Conway's Game of Life** with a color decay system.")]
        public async Task RunLifeDecayAsync(int width, int height, int duration, ulong decayLength, int delay = 100)
        {// int width, int height, 
            string path = $"../tmp/{Context.User.Id}_cgol.gif";
            GammaPalette colors = GammaPalette.Alconia;
            Grid<ConwayCell> pattern = ConwayRenderer.GetRandomPattern(width, height);
            ConwayRenderer simulator = new ConwayRenderer(GammaPalette.GammaGreen[Gamma.Standard], colors[Gamma.Min], decayLength, pattern);
            simulator.ActiveColor = GammaPalette.GammaGreen[Gamma.Max];
            List<Grid<Color>> rawFrames = simulator.Run(duration);

            await SendGifAsync(path, rawFrames, delay);
        }


        [Command("animatetime")]
        public async Task CycleTimeAsync(int framesPerHour = 1, int delay = 150, int? loop = null)
        {
            string path = $"../tmp/{Context.User.Id}_time.gif";
            FontFace font = JsonHandler.Load<FontFace>(@"../assets/fonts/orikos.json");
            char[][][][] charMap = JsonHandler.Load<char[][][][]>(@"../assets/char_map.json", new JsonCharArrayConverter());
            TextFactoryConfig config = TextFactoryConfig.Default;
            config.CharMap = charMap;

            var frames = new List<Stream>();
            var properties = new ImageProperties
            {
                TrimEmptyPixels = true,
                Padding = new Padding(2),
                Width = 47
            };

            float t = 0.00f;

            using (var factory = new TextFactory(config))
            {
                factory.SetFont(font);

                for (float h = 0; h < 24 * framesPerHour; h++)
                {
                    GammaPalette colors = TimeCycle.FromHour(t);
                    properties.Matte = colors[Gamma.Min];

                    frames.Add(DrawFrame(factory, t.ToString("00.00H"), colors[Gamma.Max], properties));

                    t += (1.00f / framesPerHour);
                }
            }

            var gifStream = new MemoryStream();
            using (var encoder = new GifEncoder(gifStream, repeatCount: loop))
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

        [Command("drawtimeat")]
        [Summary("Draws the time at the specified hour.")]
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
                TextFactoryConfig config = TextFactoryConfig.Default;
                config.CharMap = charMap;
                var properties = new ImageProperties
                {
                    TrimEmptyPixels = true,
                    Padding = new Padding(2),
                    Matte = colors[Gamma.Min]
                };

                using (TextFactory factory = new TextFactory(config))
                    using (Bitmap bmp = factory.DrawText(hour.ToString("00.00H").ToUpper(), font, properties))
                        ImageHelper.Save(bmp, path, ImageFormat.Png);

                await Context.Channel.SendFileAsync(path);
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("drawtime")]
        [Summary("Draws the current time of day.")]
        public async Task GetTimeAsync()
        {
            try
            {
                string path = $"../tmp/{Context.User.Id}_time.png";
                GammaPalette palette = TimeCycle.FromUtcNow();

                using (GraphicsService graphics = new GraphicsService())
                {
                    Bitmap bmp = graphics.DrawText(DateTime.UtcNow.ToString("hh:mm tt").ToUpper(), Gamma.Max, palette);
                    await Context.Channel.SendImageAsync(bmp, path);
                }
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [Command("drawborder")]
        [Option(typeof(int), "width", "w")]
        [Option(typeof(int), "height", "h")]
        [Option(typeof(int), "thickness", "t")]
        [Option(typeof(BorderEdge), "edge", "e")]
        [Option(typeof(BorderAllow), "allow", "a")]
        [Option(typeof(ImmutableColor), "color", "c")]
        public async Task DrawBorderAsync(int width = 32, int height = 32, int thickness = 2, PaletteType palette = PaletteType.GammaGreen, Gamma background = Gamma.StandardDim, Gamma border = Gamma.Max, BorderEdge edge = BorderEdge.Outside, BorderAllow allow = BorderAllow.All)
        {
            /*
            var width = Context.GetOptionOrDefault("width", 32);
            var height = Context.GetOptionOrDefault("height", 32);
            var thickness = Context.GetOptionOrDefault("thickness", 2);
            var color = Context.GetOptionOrDefault("color", ImmutableColor.GammaGreen);
            var edge = Context.GetOptionOrDefault("edge", BorderEdge.Outside);
            var allow = Context.GetOptionOrDefault("allow", BorderAllow.All);
            */

            using (var tmp = ImageEditor.CreateSolid(GraphicsService.GetPalette(palette)[background], width, height))
                using (Bitmap b = ImageEditor.SetBorder(tmp, GraphicsService.GetPalette(palette)[border], thickness, edge, allow))
                    await Context.Channel.SendImageAsync(b, "../tmp/border.png");
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("drawovercardmask")]
        public async Task DrawCardMaskAsync()
        {
            using (GraphicsService g = new GraphicsService())
            {
                var d = new CardDetails(Context.Account, Context.User);

                using (Bitmap card = g.DrawCard(d, PaletteType.GammaGreen))
                {
                    Grid<float> mask = ImageEditor.GetOpacityMask(card);

                    using (Bitmap gradient = ImageEditor.CreateGradient(GammaPalette.GammaGreen, card.Width, card.Height, 0.0f, GradientColorHandling.Snap))
                        using (Bitmap result = ImageEditor.SetOpacityMask(gradient, mask))
                            await Context.Channel.SendImageAsync(result, $"../tmp/{Context.User.Id}_gradient_card_mask.png");
                }
            }
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("drawcardmask")]
        public async Task DrawMaskAsync()
        {
            using (GraphicsService g = new GraphicsService())
            {
                var d = new CardDetails(Context.Account, Context.User);

                using (Bitmap card = g.DrawCard(d, PaletteType.GammaGreen))
                {
                    using (TextFactory factory = new TextFactory())
                    {
                        Grid<float> mask = ImageEditor.GetOpacityMask(card);

                        var pixels = new Grid<Color>(card.Size, new ImmutableColor(0, 0, 0, 255));

                        // TODO: Make ImmutableColor.Empty values
                        pixels.SetEachValue((x, y) =>
                            ImmutableColor.Merge(new ImmutableColor(0, 0, 0, 255), new ImmutableColor(255, 255, 255, 255),
                                mask.GetValue(x, y)));

                        using (Bitmap masked = ImageEditor.CreateRgbBitmap(pixels.Values))
                            await Context.Channel.SendImageAsync(masked, $"../tmp/{Context.User.Id}_card_mask.png");
                    }
                }
            }
        }



        [RequireUser(AccountHandling.ReadOnly)]
        [Priority(0)]
        [Command("drawcard")]
        public async Task GetCardAsync(SocketUser user = null)
        {
            user ??= Context.User;
            if (!Context.Container.TryGetUser(user.Id, out User account))
            {
                await Context.Channel.ThrowAsync("The specified user does not have an existing account.");
                return;
            }

            try
            {
                using (GraphicsService graphics = new GraphicsService())
                {
                    CardDetails d = new CardDetails(account, user);

                    CardProperties p = CardProperties.Default;
                    p.Palette = PaletteType.Glass;
                    p.Trim = false;
                    p.Casing = Casing.Upper;

                    Bitmap card = graphics.DrawCard(d, p);

                    await Context.Channel.SendImageAsync(card, $"../tmp/{Context.User.Id}_card.png");
                }
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Priority(1)]
        [Command("drawcard")]
        public async Task GetCardAsync(bool trim, CardDeny deny, BorderAllow border,
            Casing casing, FontType font, PaletteType palette, Gamma usernameGamma = Gamma.Max,
            Gamma activityGamma = Gamma.Max, Gamma borderGamma = Gamma.Max, ImageScale scale = ImageScale.Medium, int padding = 2)
        {
            SocketUser user = Context.User;
            if (!Context.Container.TryGetUser(user.Id, out User account))
            {
                await Context.Channel.ThrowAsync("The specified user does not have an existing account.");
                return;
            }

            try
            {
                using (var graphics = new GraphicsService())
                {
                    var d = new CardDetails(account, user);

                    var p = new CardProperties
                    {
                        Trim = trim,
                        Deny = deny,
                        Border = border,
                        Casing = casing,
                        Font = font,
                        Palette = palette,
                        Gamma = new Dictionary<CardComponentType, Gamma?>
                        {
                            [CardComponentType.Username] = usernameGamma,
                            [CardComponentType.Activity] = activityGamma,
                            [CardComponentType.Border] = borderGamma,
                            [CardComponentType.Background] = null,
                            [CardComponentType.Avatar] = Gamma.Max
                        },
                        Padding = new Padding(padding),
                        Scale = scale
                    };

                    Bitmap card = graphics.DrawCard(d, p);

                    Logger.Debug("Drawing card...");
                    await Context.Channel.SendImageAsync(card, $"../tmp/{Context.User.Id}_card.png");
                }
            }
            catch (Exception ex)
            {
                await Context.Channel.CatchAsync(ex);
            }
        }

        private Stream DrawFrame(TextFactory graphics, string content, Color color, ImageProperties properties = null)
        {
            MemoryStream stream = new MemoryStream();
            using (Bitmap frame = graphics.DrawText(content, color, properties))
            {
                frame.Save(stream, ImageFormat.Png);
            }

            return stream;
        }
    }
}
