using Discord.WebSocket;
using Orikivo.Utility;
using System;
using System.Threading.Tasks;

namespace Orikivo.Modules
{
    public class ModuleManager
    {
        /// <summary>
        /// Attempt to execute a specified Task, catching any errors that occur.
        /// </summary>
        public static async Task TryExecute(ISocketMessageChannel channel, Task task)
        {
            try
            {
                await Task.Run(() => task);
            }
            catch (Exception exception)
            {
                await channel.CatchAsync(exception);
            }
        }
    }
}