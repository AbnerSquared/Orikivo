using Discord.Commands;
using Orikivo.Drawing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Orikivo
{
    [Name("Debug")]
    [Summary("A collection of commands used to debug internal features.")]
    public class DebugModule : OriModuleBase<OriCommandContext>
    {
        public DebugModule() { }

        [Command("report"), Priority(0)]
        [Summary("Get the **Report** submitted with the corresponding id.")]
        public async Task GetReportAsync(int id)
        {
            await Context.Channel.SendMessageAsync(Context.Global.Reports.Contains(id) ? Context.Global.Reports[id].ToString() : $"> I could not find any reports matching #{id}.");
        }

        /*
        [Cooldown(300)]
        [RequireUser]
        [ArgSeparator(',')]
        [Command("report"), Priority(1)]
        [Summary("Create a **Report** for a specified **Command**.")]
        public async Task ReportAsync([Summary("The **Command** to report.")]string context, string title, string content, params ReportTag[] tags)
        {
            _help ??= new InfoService(_commandService, Context.Global, Context.Server);
            ContextSearchResult result = _help.GetContext(context);

            if (result.IsSuccess && (result.Type == InfoType.Command || result.Type == InfoType.Overload))
            {
                int id = Context.Global.Reports.Open(Context.Account, result.Type == InfoType.Command ? (result.Value as CommandNode).Default :
                    (result.Value as OverloadNode), new ReportBody(title, content), tags);
                await Context.Channel.SendMessageAsync($"> **Report** #{id} has been submitted.");
                return;
            }
            await Context.Channel.SendMessageAsync("> I could not find any ContextValue objects matching your context.");
        }*/

        [Command("reports")]
        public async Task ReportsAsync()
        {
            await Context.Channel.SendMessageAsync($"> There are {Context.Global.Reports.Count} reports.");
        }

        [Name("Graphics")]
        [Summary("Debugger commands that utilizes graphical input.")]
        public class GraphicsModule : OriModuleBase<OriCommandContext>
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
            [Command("drawstring")]
            [Summary("Draws a string from the provided body of text.")]
            [Option(typeof(FontType), "font")]
            [Option(typeof(PaletteType), "palette")]
            [Option(typeof(Gamma), "background")]
            [Option(typeof(Padding), "padding")]
            public async Task DrawTextAsync([Remainder]string content)
            {
                // new CanvasOptions { UseNonEmptyWidth = true, Padding = new Padding(2), BackgroundColor = new OriColor(0x0C525F) }
                // TODO: Implment OptionAttribute parsing

                using (Bitmap bmp = _graphics.DrawString(content))
                    await Context.Channel.SendImageAsync(bmp, $"../tmp/{Context.User.Id}_string.png");
            }
        }

        [Name("Filters")]
        [Summary("Debugger commands that focus on testing message filtering methods.")]
        public class FilterModule : OriModuleBase<OriCommandContext>
        {

        }
    }
}