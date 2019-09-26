using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Utility;

namespace Orikivo.Modules
{
    [Name("Plaza")]
    [Summary("The world that thrives within Orikivo.")]
    [DontAutoLoad]
    public class PlazaModule : ModuleBase<OrikivoCommandContext>
    {
        //[Command("stores"), Alias("shops", "shl")]
        [Summary("Get the status of each current shop.")]
        public async Task StoreCollectionResponseAsync()
            => await ModuleManager.TryExecute(Context.Channel, PlazaService.CheckStoresAsync(Context));

        // Submodules
        [Name("Pad")]
        [Summary("Your own place of hope.")]
        [DontAutoLoad]
        public class CenterModule : ModuleBase<OrikivoCommandContext>
        {
            [Command("storage"), Alias("inventory", "inv", "stg")]
            [Summary("Look at all of the currently owned items you have.")]
            public async Task CheckStorageAsync()
            {

            }
        }
    }

    public static class PlazaService
    {
        // user can dropoff items
        // user can pickup items

        // check user inventory..?
        public static async Task CheckStorageAsync()
        {

        }

        public static async Task CheckStoresAsync(OrikivoCommandContext Context)
        {
            MessageBuilder mb = ShopSystem.CheckStores();
            await Context.Channel.SendSourceAsync(mb.Build());
        }
    }
}
