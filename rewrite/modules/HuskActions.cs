using Discord.Commands;
using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public class HuskActions : OriModuleBase<OriCommandContext>
    {
        // this is what starts the entire game
        [RequireHusk(HuskState.Uninitialized)]
        [Command("awaken")]
        public async Task StartGameAsync()
        {

        }
    }
}
