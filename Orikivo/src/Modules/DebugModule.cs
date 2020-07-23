using Discord.Commands;
using Orikivo.Drawing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Orikivo.Modules
{
    [Name("Debug")]
    [Summary("A collection of commands used to debug internal features.")]
    public class DebugModule : OriModuleBase<DesyncContext>
    {
        public DebugModule() { }

        [Name("Graphics")]
        [Summary("Debugging commands that utilizes graphical input.")]
        public class GraphicsModule : OriModuleBase<DesyncContext>
        {
            private readonly GraphicsService _graphics;
            public GraphicsModule(GraphicsService graphics)
            {
                _graphics = graphics;
            }

            // In short, the options have to be written right after the command call

            // command [options...] <parameters>
            // command --option value --option2 value params
            // command -opt value -o value params
            // <prefix> <command> <options> <params>
            // When parsing the options, for each successful one, you remove
            [Command("drawtext")]
            [Summary("Renders a string using the **Orikivo.Drawing.TextFactory** class.")]
            [Option(typeof(FontType), "font", "f")]
            [Option(typeof(PaletteType), "palette", "P")]
            [Option(typeof(Gamma), "background", "b")]
            [Option(typeof(Padding), "padding", "p")]
            [Option("monospace", "m")]
            public async Task DrawTextAsync([Remainder]string input)
            {
                var properties = new ImageProperties();

                // new CanvasOptions { UseNonEmptyWidth = true, Padding = new Padding(2), BackgroundColor = new OriColor(0x0C525F) }
                // TODO: Implment OptionAttribute parsing

                using (Bitmap bmp = _graphics.DrawText(input))
                    await Context.Channel.SendImageAsync(bmp, $"../tmp/{Context.User.Id}_string.png");
            }
        }

        [Name("Filters")]
        [Summary("Debugging commands that focus on testing message filtering methods.")]
        public class FilterModule : OriModuleBase<DesyncContext>
        {

        }
    }
}