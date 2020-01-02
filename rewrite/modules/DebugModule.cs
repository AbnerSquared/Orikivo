using Discord.Commands;
using System.Threading.Tasks;

namespace Orikivo
{
    [Name("Debug")]
    [Summary("A collection of commands used to debug internal features.")]
    public class DebugModule : OriModuleBase<OriCommandContext>
    {
        public DebugModule() { }

        [Command("timef")]
        [Summary("Returns the current time artwork from the specified hour.")]
        public async Task GetTimeFromHourAsync([Summary("The exact hour of time to check, ranging from 0.00h to 23.99h.")]float hour)
        {

        }
    }
}