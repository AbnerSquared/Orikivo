using Discord.WebSocket;
using Orikivo;
using Orikivo.Framework;
using System;
using System.Threading.Tasks;
using Arcadia.Multiplayer;
using Microsoft.Extensions.Configuration;
using Discord.Interactions;
using Arcadia.Services;
using Discord;
using Format = Orikivo.Format;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Arcadia.Interactions;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia
{
    public class InteractionHandler
    {
        public static readonly TimeSpan GlobalCooldown = TimeSpan.FromSeconds(2);
        public static readonly TimeSpan CommandNoticeCooldown = TimeSpan.FromSeconds(0.75);
        public static readonly TimeSpan CooldownTickDuration = TimeSpan.FromSeconds(3); // Go down a level every 3 seconds
        private readonly TimeSpan UpdateCooldown = TimeSpan.FromSeconds(10);
        public static readonly int CommandLimitPerSecond = 3; // 3 times in a second = LEVEL UP

        private DateTime LastGuildCountUpdate { get; set; }

        private readonly InteractionService _service;
        private readonly DiscordSocketClient _client;
        private readonly ArcadeContainer _container;
        private readonly IServiceProvider _provider;
        private readonly GameManager _games;
        private readonly IConfigurationRoot _config;

        public InteractionHandler(DiscordSocketClient client, InteractionService service, ArcadeContainer container, IServiceProvider provider, GameManager games, IConfigurationRoot config)
        {
            _container = container;
            _provider = provider;
            _config = config;
            _games = games;

            _client = client;
            _client.Log += Logger.LogAsync;
            _client.GuildAvailable += SumGuildCount;
            _client.JoinedGuild += UpdateGuildCount;
            _client.LeftGuild += UpdateGuildCount;
            _client.Ready += OnReady;
            _client.Disconnected += ResetGuildCount;
            _client.InteractionCreated += HandleInteractionAsync;


            _service = service;
            _service.Log += Logger.LogAsync;
            
            _service.InteractionExecuted += OnExecutedAsync;
        }

        private int GuildCount { get; set; }
        private int PostedCount { get; set; }

        private async Task ResetGuildCount(Exception error)
        {
            GuildCount = 0;
        }

        private async Task SumGuildCount(SocketGuild guild)
        {
            GuildCount++;
        }

        private async Task<int> GetGuildCountAsync()
        {
            if (GuildCount == 0)
                return (await _client.Rest.GetGuildsAsync()).Count;

            Logger.Debug($"Incremented: {GuildCount}");

            return GuildCount;
        }

        private async Task UpdateGuildCount(SocketGuild guild)
        {
            if (string.IsNullOrWhiteSpace(_config["token_dbl"]))
                return;

           
            //DblClient ??= new DblClient(_client.CurrentUser.Id, _config["token_dbl"]);

            if (DateTime.UtcNow - LastGuildCountUpdate < UpdateCooldown)
                return;

            //await DblClient.UpdateStatsAsync((await _client.Rest.GetGuildsAsync()).Count).ConfigureAwait(false);
            LastGuildCountUpdate = DateTime.UtcNow;
        }

        private async Task OnReady()
        {
            // All of the commands that WE can execute are returned with this method.
            // We can use these to get command IDs and so forth
            await _service.AddModulesGloballyAsync(true, _service.Modules.ToArray());
            // await _service.AddModulesToGuildAsync(Orikivo.Constants.SupportId, modules: _service.Modules.ToArray());
        }

        private static string WriteCooldownText(DateTime expiry, bool isGlobal = true)
        {
            var text = new StringBuilder();

            text.AppendLine($"> 🔻 {(isGlobal ? "You are executing commands too quickly." : "This command is on cooldown.")}");
            text.AppendLine($"> {(isGlobal ? "Please wait" : "Try again in")} {Format.Counter(DateTime.UtcNow - expiry)}.");

            return text.ToString();
        }

        public async Task HandleInteractionAsync(SocketInteraction arg)
        {
            // Ignore bots
            if (arg.User.IsBot)
                return;

            // ignore any commands in a reserved channel ONLY if the user is currently in a game
            if (_games.ReservedUsers.ContainsKey(arg.User.Id) && _games.ReservedChannels.ContainsKey(arg.Channel.Id))
                return;

            var ctx = new ArcadeInteractionContext(_client, _container, arg);
            await ExecuteAsync(ctx);
        }


        public async Task ExecuteAsync(ArcadeInteractionContext ctx)
        {
            if (ctx.Account != null)
            {
                if (ctx.Account.GlobalCooldown.HasValue)
                {
                    if (!(DateTime.UtcNow - ctx.Account.GlobalCooldown >= TimeSpan.Zero))
                    {
                        if ((ctx.Server?.Config.AllowCooldownNotices ?? true) && !ctx.Account.HasBeenNoticed)
                        {
                            await ctx.Interaction.FollowupAsync(WriteCooldownText(ctx.Account.GlobalCooldown.Value), ephemeral: true);
                            ctx.Account.HasBeenNoticed = true;
                        }

                        return;
                    }

                    ctx.Account.HasBeenNoticed = false;
                }

                if (ctx.Account != null)
                {
                   // string id = ContextNode.GetId(ctx.Interaction);
                   // Console.WriteLine(id);
//
                   // if (ctx.Account.InternalCooldowns.ContainsKey(id))
                   // {
                   //     if (!(DateTime.UtcNow - ctx.Account.InternalCooldowns[id] >= TimeSpan.Zero))
                   //     {
                    //        await ctx.Channel.SendMessageAsync(WriteCooldownText(ctx.Account.InternalCooldowns[id], false));
                    //        ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);
                    //        return;
                    //    }
                    //}

                    //IEnumerable<RequireItemAttribute> requiredItems = possibleCommand.Attributes.OfType<RequireItemAttribute>();

                   // foreach (RequireItemAttribute criterion in requiredItems)
                   // {
                  //      if (ItemHelper.GetOwnedAmount(ctx.Account, criterion.ItemId) < criterion.Amount)
                   //     {
                   //         string message = Check.NotNull(criterion.OnFail)
                   //             ? criterion.OnFail
                   //             : Format.Warning($"You are missing a required item (**{ItemHelper.NameOf(criterion.ItemId)}**) needed to use this command.");
                  //          await ctx.Channel.SendMessageAsync(message, flags: MessageFlags.Ephemeral);
                  //          return;
                   //     }
                   // }

                    //if ((possibleCommand.Attributes.FirstOrDefault<SessionAttribute>() != null
                    //    || possibleCommand.Attributes.FirstOrDefault<RequireNoSessionAttribute>() != null)
                   //     && ctx.Account != null && ctx.Account.IsInSession)
                    //{
                    //    await ctx.Channel.SendMessageAsync(Format.Warning("You are currently in a session."), flags: MessageFlags.Ephemeral);
                   //     ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);
                   //     return;
                   // }

                    //if (possibleCommand.Attributes.FirstOrDefault<SessionAttribute>() != null)
                    //{
                   //     if (ctx.Account != null)
                    //        ctx.Account.IsInSession = true;
                    //}

                    if (ctx.Account != null)
                        ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(GlobalCooldown);
                }
                //}
            }

            if (ctx.Account != null && ctx.Account.GetVar(Vars.CurrentMonthYear) != ctx.Data.Data.CurrentMonthYear)
            {
                ctx.Account.SetVar(Vars.CurrentMonthYear, ctx.Data.Data.CurrentMonthYear);
                await ClearMonthlyData(ctx.Account);
            }

            // TODO: Use the command that was found and execute that instead to reduce command complexity
            await _service.ExecuteCommandAsync(ctx, _provider);
        }

        private async Task ClearMonthlyData(ArcadeUser user)
        {
            Var.ClearAll(user, x => x.Key.StartsWith("monthly", StringComparison.OrdinalIgnoreCase));
        }

        private async Task OnExecutedAsync(ICommandInfo command, IInteractionContext context, IResult result)
        {
            // TODO: Make the specific context exchangeable.
            //Logger.Debug($"Executed {command.Value?.Name}");

            if (!(context is ArcadeInteractionContext ctx))
                throw new Exception("Invalid context provided.");

            // Remove any session locks IF the command that was executed invoked a session call
            if (command.Attributes.FirstOrDefault<SessionAttribute>() != null && ctx.Account != null)
            {
                ctx.Account.IsInSession = false;
            }

            // Attempt to set a global cooldown on the account that executed this command

            // If the command failed
            if (!result.IsSuccess)
            {
                if (result is ExecuteResult execute)
                {
                    if (!result.IsSuccess)
                        await context.Channel.CatchAsync(execute.Exception, ctx?.Account?.Config?.ErrorHandling ?? StackTraceMode.Simple);
                }
                else
                {
                    if (result.Error == InteractionCommandError.UnknownCommand)
                        return;

                    if (result.Error == InteractionCommandError.UnmetPrecondition)
                    {
                        await context.Interaction.FollowupAsync(Format.Warning(result.ErrorReason ?? "An unmet precondition was found when executing this command."), ephemeral: true);
                        return;
                    }

                    if (result.Error == InteractionCommandError.ConvertFailed)
                    {
                        await context.Interaction.FollowupAsync($"> {Icons.Warning} **Odd.**\n> {result.ErrorReason ?? "The input specified could not be converted or found by the typereader."}", ephemeral: true);
                        return;
                    }

                    await context.Interaction.RespondWithErrorAsync(Format.Error("Oops!", "An error has occurred.", result.ErrorReason), ephemeral: true);
                    // await context.Channel.ThrowAsync(result.ErrorReason);
                }

                return;
            }

            await UpdateAsync(command, ctx);
        }

        // update all accounts and users accordingly based on the command
        private async Task UpdateAsync(ICommandInfo command, ArcadeInteractionContext ctx)
        {
            var cooldown = command.Attributes.FirstOrDefault<CooldownAttribute>();

            // If the cooldown and account both exist
            //if (cooldown != null && ctx.Account != null)
            //{
            //    string id = ContextNode.GetId(command, true);

            //    if (!ctx.Account.InternalCooldowns.TryAdd(id, DateTime.UtcNow.Add(cooldown.Duration)))
            //        ctx.Account.InternalCooldowns[id] = DateTime.UtcNow.Add(cooldown.Duration);
            //}

            //if (ctx.Account != null)
            //   ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(GlobalCooldown);

            // Check if the user was updated or doesn't exist to save
            var requireUser = command.Preconditions.FirstOrDefault<RequireUserAttribute>();

            if (requireUser != null && requireUser.Handling != AccountHandling.ReadOnly)
            {
                MeritHelper.UnlockAvailable(ctx.Account);
                ResearchHelper.TryCompleteResearch(ctx.Account);

                Logger.Debug("User updated. Now saving...");
                ctx.Data.Users.TrySave(ctx.Account);
            }
            else if (ctx.Account != null && !JsonHandler.JsonExists<ArcadeUser>(ctx.User.Id))
            {
                ctx.Data.Users.TrySave(ctx.Account);
            }

            // Check if the guild was updated or doesn't exist to save
            var requireGuild = command.Preconditions.FirstOrDefault<RequireGuildAttribute>();

            if (requireGuild?.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly) ?? false)
            {
                ctx.Data.Guilds.TrySave(ctx.Server);
            }
            else if (ctx.Guild != null && ctx.Server != null && !JsonHandler.JsonExists<BaseGuild>(ctx.Guild.Id))
            {
                if (ctx.Guild == null)
                    ctx.GetOrAddGuild(ctx.Guild);

                ctx.Data.Guilds.TrySave(ctx.Server);
            }

            var requireGlobal = command.Attributes.FirstOrDefault<RequireGlobalDataAttribute>();

            if (requireGlobal != null)
                ctx.Data.SaveGlobalData();
        }
    }
}
