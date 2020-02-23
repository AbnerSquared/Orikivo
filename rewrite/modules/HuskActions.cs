using Discord.Commands;
using Discord.WebSocket;
using Orikivo.Unstable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    [Hide]
    [Name("Actions")]
    [Summary("A collection of actions that can be executed as a Husk.")]
    public class HuskActions : OriModuleBase<OriCommandContext>
    {
        private readonly CommandService _commands;
        public HuskActions(CommandService commands)
        {
            _commands = commands;
        }

        // this is what starts the entire husk system
        // this can only be used on people who have never started.
        [RequireUser]
        [CheckFlags(Gate.NOT, HuskFlags.Initialized)]
        [Command("awaken")]
        [Summary("Awaken your **Husk** in the physical world for the first time.")]
        public async Task AwakenAsync()
        {
            WorldEngine.Initialize(Context.Account);

            StringBuilder summary = new StringBuilder();
            summary.AppendLine(Context.Account.Husk.Location.GetSummary());
            summary.Append("You are free to explore.");

            await Context.Channel.SendMessageAsync(summary.ToString());

        }

        [RequireUser(AccountHandling.ReadOnly)]
        [Command("relationships"), Alias("relations")]
        [Summary("View a quick summary about the relations of everybody you know.")]
        public async Task GetRelationsAsync()
        {
            StringBuilder relations = new StringBuilder();

            if (Context.Account.Brain.Relations.Count == 0)
                await Context.Channel.SendMessageAsync("You don't seem to know anyone. Get out there and talk to others!");
            else
            {
                relations.AppendLine("Relationships:");
                relations.AppendJoin("\n", Context.Account.Brain.Relations.Select(x => $"`{x.Key}` [**{Relationship.GetLevel(x.Value).ToString()}**]"));

                await Context.Channel.SendMessageAsync(relations.ToString());
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
            if (!WorldEngine.CanMove(Context.Account.Husk))
            {
                await Context.Channel.SendMessageAsync("You are currently in transit and cannot perform any actions.");
                return;
            }

            await Context.Channel.SendMessageAsync(WorldEngine.ShowLocations(Context.Account.Husk));
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
        [RequireLocation(LocationType.Construct | LocationType.Area)]
        [Command("leave")]
        [Summary("Leave the current **Location** you are in.")]
        public async Task LeaveConstructAsync()
        {
            if (WorldEngine.TryLeave(Context.Account.Husk))
            {
                await Context.Channel.SendMessageAsync($"You are now in **{Context.Account.Husk.Location.GetInnerName()}**.");
            }
        }

        [RequireUser]
        [RequireLocation(LocationType.Area | LocationType.Construct)]
        [Command("chat"), Priority(0)]
        [Summary("Shows all available NPCs you are able to chat with.")]
        public async Task ShowNpcsAsync()
        {
            await Context.Channel.SendMessageAsync(WorldEngine.ShowNpcs(Context.Account.Husk));
        }

        // TODO: Create Chat command for Sectors, so that if an NPC was seen inside a view radius, 
        // and the player wished to talk with them, perform a "catching up" kind of system,
        // where the player would run towards the NPC for the specified estimated time.
        // likewise, prevent chatting with an npc, if their travel speed is greater than yours, since
        //  you would never be able to catch up.
        [RequireUser]
        [RequireLocation(LocationType.Area | LocationType.Construct)]
        [Command("chat"), Priority(1)]
        [Summary("Initiates conversation with a specified NPC.")]
        public async Task ChatAsync(string id)
        {
            if (WorldEngine.CanChat(Context.Account.Husk, id, out Npc npc))
            {
                // TODO: Handle how dialogue pools are chosen.
                ChatHandler chat = new ChatHandler(Context, npc, WorldEngine.NextPool());
                await HandleChatAsync(chat);
            }
            else
            {
                await Context.Channel.SendMessageAsync("Unable to determine specified individual. Please refer to our NPC tracker (`chat`).");
            }
        }

        [RequireUser]
        [RequireLocation(ConstructTag.Market)]
        [Command("shop")]
        [Summary("Go to the current **Vendor** and begin shopping.")]
        public async Task ShopAsync()
        {
            if (WorldEngine.CanShop(Context.Account.Husk, out Market market))
            {
                ShopHandler shop = new ShopHandler(Context, market, PaletteType.Glass);
                await HandleShopAsync(shop);
            }
            else
            {
                await Context.Channel.SendMessageAsync("An error has occurred when initializing the shop.");
            }
        }

        [RequireUser(AccountHandling.ReadOnly)]
        [CheckFlags(Gate.HAS, HuskFlags.Initialized)]
        [Command("actions"), Alias("act")]
        public async Task GetActionsAsync()
        {
            InfoService info = new InfoService(_commands, Context.Global, Context.Server);

            await Context.Channel.SendMessageAsync(info.GetActions(Context.Account));
        }

        [RequireLocation(ConstructTag.Market)]
        [Command("schedule")]
        [Summary("Gets the current schedule in use for this **Market**.")]
        public async Task GetScheduleAsync()
        {
            await Context.Channel.SendMessageAsync(((Market)Context.Account.Husk.Location.GetConstruct()).ShowSchedule());
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

                Context.Account.Brain.AddOrUpdateRelationship(handler.Relationship);
            }
            catch (Exception e)
            {
                await Context.Channel.CatchAsync(e);
            }
        }

        // this will pull up all available NPCs and Constructs in your view radius on the map.
        // for NPCs in a sector, a Position is specified.
        [RequireUser]
        [RequireLocation(LocationType.Sector)]
        [Summary("Explore and look around the **Sector** you are currently in.")]
        public async Task ExploreAsync()
        {

        }


        [RequireUser]
        [RequireLocation(LocationType.Area)]
        [Summary("View a list of all available **Areas** you can travel to.")]
        public async Task TravelAsync()
        {
            // Available Areas (Sector 0)
            // 
        }

        // because free roaming is difficult, i want to incorporate a system.
        // start the user at the Area.EntranceDirection.
        //


        [RequireUser]
        [RequireLocation(LocationType.Area)]
        [Summary("Travel to a specified **Area**. (Travel time is calculated)")]
        public async Task TravelAsync(string id)
        {

        }

        [RequireUser]
        [RequireLocation(LocationType.Area | LocationType.Sector)]
        [Command("goto"), Priority(1)] // This is used to travel to any viable location based on its current location.
        [Summary("Travel to a specified **Construct**.")]
        public async Task GoToAsync(string id)
        {
            if (!WorldEngine.CanMove(Context.Account.Husk))
            {
                await Context.Channel.SendMessageAsync("You are currently in transit, and cannot perform any actions.");
                return;
            }

            if (Context.Account.Husk.Location.GetInnerType() == LocationType.Sector)
            {
                TravelResult result = WorldEngine.TryGoTo(Context.Account.Husk, id, out Area attempted);

                switch (result)
                {
                    case TravelResult.Start:
                        StringBuilder travel = new StringBuilder();
                        travel.AppendLine($"Now travelling to **{attempted.Name}**.");
                        MovementInfo info = Context.Account.Husk.Movement;

                        travel.Append($"Expected Arrival: {OriFormat.GetShortTime((info.Arrival - info.StartedAt).TotalSeconds)}");
                        await Context.Channel.SendMessageAsync(travel.ToString());
                        break;
                    case TravelResult.Success:
                        if (attempted.Image != null)
                            await Context.Channel.SendFileAsync(attempted.Image.Path, $"Welcome to **{attempted.Name}**.");
                        else
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
                TravelResult result = WorldEngine.TryGoTo(Context.Account.Husk, id, out Construct attempted);

                switch (result)
                {
                    case TravelResult.Success:
                        if (attempted.Image != null)
                            await Context.Channel.SendFileAsync(attempted.Image.Path, $"Welcome to **{attempted.Name}**.");
                        else
                            await Context.Channel.SendMessageAsync($"Welcome to **{attempted.Name}**.");
                        break;
                    case TravelResult.Closed:
                        if (attempted.Tag.HasFlag(ConstructTag.Market))
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
    }
}
