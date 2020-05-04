using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Desync;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    [Ignore]
    [Name("Actions")]
    [Summary("A collection of actions that can be executed as a Husk.")]
    public class Actions : OriModuleBase<OriCommandContext>
    {
        private readonly CommandService _commands;
        public Actions(CommandService commands)
        {
            _commands = commands;
        }

        [RequireUser]
        //[OnlyWhen(LogicGate.NAND, HuskFlags.Initialized)]
        [CheckFlags(Gate.NOT, DesyncFlags.Initialized)]
        [Command("awaken")]
        [Summary("Awaken your **Husk** in the physical world for the first time.")]
        public async Task AwakenAsync()
        {
            Engine.Initialize(Context.Account);

            StringBuilder summary = new StringBuilder();
            summary.AppendLine(Context.Account.Husk.Location.Summarize());
            summary.Append("You are free to explore.");

            await Context.Channel.SendMessageAsync(Context.Account, summary.ToString());

        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("relationships"), Alias("relations")]
        [Summary("View a quick summary about the relations of everybody you know.")]
        public async Task GetRelationsAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            StringBuilder relations = new StringBuilder();

            if (Context.Account.Brain.Relations.Count == 0)
                await Context.Channel.SendMessageAsync(Context.Account, "You don't seem to know anyone. Get out there and talk to others!");
            else
            {
                relations.AppendLine("Relationships:");
                relations.AppendJoin("\n", Context.Account.Brain.Relations.Select(x => $"`{x.Key}` [**{AffinityData.GetLevel(x.Value).ToString()}**]"));

                await Context.Channel.SendMessageAsync(Context.Account, relations.ToString());
            }
        }

        // this command is meant for leaving Constructs, from which the layer is 0.
        // [Command("exit")]

        // If no ID was given, show a list of IDs that the user can travel to in their current position.
        // This is only if they are NOT in a construct
        [RequireLocation(LocationType.Area | LocationType.Sector)]
        [Command("goto"), Priority(0)] // This is used to travel to any viable location based on its current location.
        [Summary("View a list of available locations you can travel to in your current **Area**.")]
        public async Task GoToAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            if (!Engine.CanMove(Context.Account, Context.Account.Husk))
            {
                await Context.Channel.SendMessageAsync("You are currently in transit and cannot perform any actions.");
                return;
            }

            await Context.Channel.SendMessageAsync(Engine.ShowLocations(Context.Account.Husk, Context.Account.Brain));
        }

        // Exploring is only while your within a sector at a minimum.
        // this is short for leaving the sector, into the wild areas (FIELDs).
        // travel time is calculated based on where the ENTRANCE is for the SECTOR.
        // [Command("explore")]


        // You can only chat with locations that support NPCs.
        // AREAs and CONSTRUCTs can have NPCs.
        // if there is nobody to talk to, you will be notified.
        // if an ID isn't specified, you will be shown the list of NPCs travelling around the area.
        // [Command("chat")]

        [RequireUser]
        // [BindToRegion(LocationType.Construct | LocationType.Area)]
        [RequireLocation(LocationType.Construct | LocationType.Area)]
        [Command("leave")]
        [Summary("Leave the current **Location** you are in.")]
        public async Task LeaveConstructAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            if (Engine.TryLeave(Context.Account.Husk))
            {
                await Context.Channel.SendMessageAsync($"You are now in **{Context.Account.Husk.Location.GetInnerName()}**.");
            }
        }

        [RequireUser]
        [BindToRegion(RegionType.Structure)]
        [Command("identify")]
        [Summary("Determine the **Structure** you are currently at.")]
        public async Task IdentifyAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            if (!Engine.IsNearClosestStructure(Context.Account.Husk, out Structure structure))
            {
                await Context.Channel.SendMessageAsync("You are not near a structure.");
                return;
            }

            if (Engine.TryIdentifyStructure(Context.Account.Husk, Context.Account.Brain, structure))
                await Context.Channel.SendMessageAsync($"You have identified this structure as **{structure.Name}**.");
            else
                await Context.Channel.SendMessageAsync($"You have already identified **{structure.Name}**");
        }

        [RequireUser]
        // [BindToRegion(LocationType.Construct | LocationType.Area)]
        [RequireLocation(LocationType.Area | LocationType.Construct)]
        [Command("chat"), Priority(0)]
        [Summary("Shows all available NPCs you are able to chat with.")]
        public async Task ShowNpcsAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            await Context.Channel.SendMessageAsync(Context.Account, Engine.ShowNpcs(Context.Account.Husk));
        }

        // TODO: Create Chat command for Sectors, so that if an NPC was seen inside a view radius, 
        // and the player wished to talk with them, perform a "catching up" kind of system,
        // where the player would run towards the NPC for the specified estimated time.
        // likewise, prevent chatting with an npc, if their travel speed is greater than yours, since
        //  you would never be able to catch up.
        [RequireUser]
        // [BindToRegion(LocationType.Construct | LocationType.Area)]
        [RequireLocation(LocationType.Area | LocationType.Construct)]
        [Command("chat"), Priority(1)]
        [Summary("Initiates conversation with a specified NPC.")]
        public async Task ChatAsync(string id)
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            if (Engine.CanChatWithNpc(Context.Account.Husk, id, out Npc npc))
            {
                // TODO: Handle how dialogue pools are chosen.
                ChatHandler chat = new ChatHandler(Context, npc, Engine.NextPool());
                await HandleChatAsync(chat);
            }
            else
            {
                await Context.Channel.SendMessageAsync(Context.Account, "Unable to determine specified individual. Please refer to our NPC tracker (`chat`).");
            }
        }

        [RequireUser]
        [Command("status")]
        [Summary("View your current status.")]
        public async Task ViewStatusAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            await Context.Channel.SendMessageAsync(HuskHandler.ViewStatus(Context.Account, Context.Account.Husk));
        }

        [RequireUser]
        [Access(AccessLevel.Dev)]
        [Command("desync")]
        [Summary("commit die, and respawn.")]
        public async Task DesyncAsync()
        {

            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You're already desynchronized.");
                return;
            }
            // TODO: implement resync distance time
            // implement item selection from backpack.

            Memorial memorial = Context.Account.Husk.GetMemorial();
            Context.Account.Husk = null;
            Context.Account.Brain.Memorials.Add(memorial);
            var now = DateTime.UtcNow;
            Context.Account.Brain.ResyncAt = now.AddSeconds(10);

            string timer = OriFormat.GetShortTime((Context.Account.Brain.ResyncAt.Value - now).TotalSeconds);
            await Context.Channel.SendMessageAsync($"You have been desychronized. You will be resynchronized in {timer}.");
        }


        [RequireUser]
        // [BindToRegion(ConstructType.Market)]
        [RequireLocation(ConstructType.Market)]
        [Command("shop")]
        [Summary("Go to the current **Vendor** and begin shopping.")]
        public async Task ShopAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            if (Engine.CanShopAtCurrentLocation(Context.Account.Husk, out Market market))
            {
                ShopHandler shop = new ShopHandler(Context, market, PaletteType.Glass);
                await HandleShopAsync(shop);
            }
            else
            {
                await Context.Channel.SendMessageAsync(Context.Account, "An error has occurred when initializing the shop.");
            }
        }

        [RequireUser(AccountHandling.ReadOnly)]
        // [OnlyWhen(LogicGate.AND, HuskFlags.Initialized)]
        [CheckFlags(Gate.HAS, DesyncFlags.Initialized)]
        [Command("actions"), Alias("act")]
        public async Task GetActionsAsync()
        {
            InfoService info = new InfoService(_commands, Context.Global, Context.Server);

            await Context.Channel.SendMessageAsync(info.GetActions(Context.Account));
        }

        // [BindToRegion(ConstructType.Market)]
        [RequireLocation(ConstructType.Market)]
        [Command("schedule")]
        [Summary("Gets the current schedule in use for this **Market**.")]
        public async Task GetScheduleAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            await Context.Channel.SendMessageAsync(((Market)(Context.Account.Husk.Location.GetLocation() as Construct)).ShowSchedule());
        }

        

        [RequireUser]
        // [BindToRegion(ConstructType.Highrise)]
        [RequireLocation(ConstructType.Highrise)]
        [Command("goup")]
        [Summary("Travel up a floor.")]
        public async Task GoUpAsync()
        {
        }

        [RequireUser]
        // [BindToRegion(ConstructType.Highrise)]
        [RequireLocation(ConstructType.Highrise)]
        [Command("godown")]
        [Summary("Travel down a floor.")]
        public async Task GoDownAsync()
        {

        }

        [RequireUser]
        [RequireLocation(LocationType.Sector | LocationType.Field | LocationType.World)]
        [Command("recover")]
        [Summary("If near a memorial, recover its belongings.")]
        public async Task RecoverAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            if (!Engine.CanMove(Context.Account, Context.Account.Husk))
            {
                await Context.Channel.SendMessageAsync("You are currently in transit and cannot perform any actions.");
                return;
            }

            foreach (Memorial memorial in Context.Account.Brain.Memorials)
            {
                if (Engine.IsNearPoint(Context.Account.Husk, memorial.Location.X, memorial.Location.Y))
                {
                    Engine.Recover(Context.Account, memorial);
                    await Context.Channel.SendMessageAsync("You have recovered your belongings.");
                    return;
                }
            }

            await Context.Channel.SendMessageAsync("You are not near a memorial.");
        }

        [RequireUser]
        [Command("backpack")]
        [Summary("View your backpack.")]
        public async Task ViewBackpackAsync()
        {
            if (!Engine.CanAct(Context.Account))
            {
                await Context.Channel.SendMessageAsync("You are currently desynchronized. Unable to establish connection.");
                return;
            }

            if (Context.Account.Husk?.Backpack?.ItemIds?.Count() == 0)
            {
                await Context.Channel.SendMessageAsync("You do not have any items in your backpack.");
                return;
            }

            var items = new StringBuilder();
            foreach(var item in Context.Account.Husk.Backpack.GetItems())
            {
                items.AppendLine($"> **{item.Name}** (x{Context.Account.Husk.Backpack.ItemIds[item.Id]})");
            }

            await Context.Channel.SendMessageAsync(items.ToString());
        }

        [RequireUser]
        // [BindToRegion(LocationType.World | LocationType.Field | LocationType.Sector)]
        [RequireLocation(LocationType.Sector | LocationType.Field | LocationType.World)]
        [Command("goto"), Priority(2)]
        [Summary("Travel to the specified coordinates.")]
        public async Task GoToAsync(int x, int y)
        {
            if (!Engine.CanMove(Context.Account, Context.Account.Husk))
            {
                await Context.Channel.SendMessageAsync("You are currently in transit and cannot perform any actions.");
                return;
            }

            TravelResult result = Engine.TryGoTo(Context.Account.Husk, x, y, out Destination attempted);

            switch (result)
            {
                case TravelResult.Start:
                    StringBuilder travel = new StringBuilder();
                    travel.AppendLine($"Now travelling to (**{attempted.X}**, **{attempted.Y}**) in **{attempted.GetInnerName()}**.");
                    Destination info = Context.Account.Husk.Destination;

                    travel.Append($"Expected Arrival: {OriFormat.GetShortTime((info.Arrival - info.StartedAt).TotalSeconds)}");
                    await Context.Channel.SendMessageAsync(travel.ToString());
                    break;
                case TravelResult.Instant:
                    var location = attempted.GetLocation();

                    if (location?.Exterior != null)
                        await Context.Channel.SendFileAsync(location.Exterior.Path, $"You have reached (**{attempted.X}**, **{attempted.Y}**).");
                    else
                        await Context.Channel.SendMessageAsync($"You have reached (**{attempted.X}**, **{attempted.Y}**).");
                    break;

                case TravelResult.Invalid:
                default:
                    await Context.Channel.SendMessageAsync("I'm sorry, but I could not pinpoint the coordinates you were referring to. Did you by any chance mistype?");
                    break;
            }

        }

        [RequireUser]
        // [BindToRegion(LocationType.Sector | LocationType.Area)]
        [RequireLocation(LocationType.Area | LocationType.Sector)]
        [Command("goto"), Priority(1)] // This is used to travel to any viable location based on its current location.
        [Summary("Travel to a specified **Construct**.")]
        public async Task GoToAsync(string id)
        {
            if (!Engine.CanMove(Context.Account, Context.Account.Husk))
            {
                await Context.Channel.SendMessageAsync("You are currently in transit and cannot perform any actions.");
                return;
            }

            if (Context.Account.Husk.Location.GetInnerType() == LocationType.Sector)
            {
                TravelResult result = Engine.TryGoToInSector(Context.Account.Husk, Context.Account.Brain, id, out Region attempted);

                switch (result)
                {
                    case TravelResult.Start:
                        StringBuilder travel = new StringBuilder();
                        travel.AppendLine($"Now travelling to **{attempted.Name}**.");
                        Destination info = Context.Account.Husk.Destination;

                        travel.Append($"Expected Arrival: {OriFormat.GetShortTime((info.Arrival - info.StartedAt).TotalSeconds)}");
                        await Context.Channel.SendMessageAsync(travel.ToString());
                        break;
                    case TravelResult.Instant:
                        //if (attempted.Exterior != null)
                        //    await Context.Channel.SendFileAsync(attempted.Exterior.Path, $"Welcome to **{attempted.Name}**.");
                        //else
                            await Context.Channel.SendMessageAsync($"Welcome to **{attempted.Name}**.");
                        break;

                    case TravelResult.Invalid:
                    default:
                        await Context.Channel.SendMessageAsync("I'm sorry, but I could not find the location you were referring to. Did you by any chance misspell?");
                        break;
                }
            }
            else
            {
                TravelResult result = Engine.TryGoToInArea(Context.Account.Husk, id, out Construct attempted);

                switch (result)
                {
                    case TravelResult.Instant:
                        if (attempted.Exterior != null)
                            await Context.Channel.SendFileAsync(attempted.Exterior.Path, $"Welcome to **{attempted.Name}**.");
                        else
                            await Context.Channel.SendMessageAsync($"Welcome to **{attempted.Name}**.");
                        break;
                    case TravelResult.Closed:
                        if (attempted.Tag.HasFlag(ConstructType.Market))
                            await Context.Channel.SendMessageAsync($"**{attempted.Name}** is currently closed. Come back in **{(((Market)attempted).GetNextBlock(DateTime.UtcNow).From - DateTime.UtcNow).ToString(@"hh\:mm\:ss")}**.");
                        else
                            await Context.Channel.SendMessageAsync($"**{attempted.Name}** is currently closed.");
                        break;

                    case TravelResult.Invalid:
                    default:
                        await Context.Channel.SendMessageAsync("I'm sorry, but I could not find the location you were referring to. Did you by any chance misspell?");
                        break;
                }
            }
        }

        private async Task HandleShopAsync(ShopHandler shop)
        {
            try
            {
                MessageCollector collector = new MessageCollector(Context.Client);

                MatchOptions options = new MatchOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = TimeSpan.FromSeconds(30),
                    Action = shop
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
        private async Task HandleChatAsync(ChatHandler handler)
        {
            try
            {
                MessageCollector collector = new MessageCollector(Context.Client);

                MatchOptions options = new MatchOptions
                {
                    ResetTimeoutOnAttempt = true,
                    Timeout = TimeSpan.FromSeconds(20),
                    Action = handler
                };

                Func<SocketMessage, int, bool> filter = delegate (SocketMessage message, int index)
                {
                    return (message.Author.Id == Context.User.Id) && (message.Channel.Id == Context.Channel.Id);
                };

                await collector.MatchAsync(filter, options);

                Context.Account.Brain.AddOrUpdateAffinity(handler.Relationship);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        }
    }
}
