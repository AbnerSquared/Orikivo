using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arcadia.Multiplayer;
using Arcadia.Services;
using DiscordBoats;
using Microsoft.Extensions.Configuration;
using Format = Orikivo.Format;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia
{
    public class CommandHandler
    {
        public static readonly TimeSpan GlobalCooldown = TimeSpan.FromSeconds(2);
        public static readonly TimeSpan CommandNoticeCooldown = TimeSpan.FromSeconds(0.75);
        // CooldownTickDuration = TimeSpan.FromSeconds(3); // Go down a level every 3 seconds
        // CooldownRatelimiter = 3 times in 2 seconds => level up

        private readonly CommandService _service;
        private readonly DiscordSocketClient _client;
        private readonly ArcadeContainer _container;
        private readonly IServiceProvider _provider;
        private readonly InfoService _info;
        private readonly GameManager _games;
        private readonly IConfigurationRoot _config;

        private readonly TimeSpan UpdateCooldown = TimeSpan.FromSeconds(10);
        private DateTime LastGuildCountUpdate { get; set; }

        public CommandHandler(DiscordSocketClient client, CommandService service, ArcadeContainer container,
            IServiceProvider provider, InfoService info, GameManager games, IConfigurationRoot config)
        {
            _container = container;
            _provider = provider;
            _config = config;
            _info = info;
            _games = games;

            _client = client;
            _client.MessageReceived += ReadInputAsync;
            _client.Log += Logger.LogAsync;
            _client.GuildAvailable += SumGuildCount;
            _client.JoinedGuild += UpdateGuildCount;
            _client.LeftGuild += UpdateGuildCount;
            _client.Ready += OnReady;
            _client.Disconnected += ResetGuildCount;

            _service = service;
            _service.Log += Logger.LogAsync;
            _service.CommandExecuted += OnExecutedAsync;
        }

        private BoatClient BoatClient { get; set; }

        // private DblClient DblClient { get; set; }

        private int GuildCount { get; set; }

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

        private async Task OnReady()
        {
            /*
            try
            {
                //if (string.IsNullOrWhiteSpace(_config["token_dbl"]))
                    return;

                //BoatClient ??= new BoatClient(_client.CurrentUser.Id, _config["token_discord_boats"]);
                //DblClient ??= new DblClient(_client.CurrentUser.Id, _config["token_dbl"]);

                if (DateTime.UtcNow - LastGuildCountUpdate < UpdateCooldown)
                    return;

                int guildCount = await GetGuildCountAsync();

                //await BoatClient.UpdateGuildCountAsync(guildCount).ConfigureAwait(false);
                await DblClient.UpdateStatsAsync(guildCount).ConfigureAwait(false);

                LastGuildCountUpdate = DateTime.UtcNow;
                Logger.Debug($"Posted guild count to Discord Boats ({guildCount:##,0})");
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString());
            }*/
        }

        private async Task UpdateGuildCount(SocketGuild guild)
        {
            if (string.IsNullOrWhiteSpace(_config["token_discord_boats"]))
                return;

            //BoatClient ??= new BoatClient(_client.CurrentUser.Id, _config["token_discord_boats"]);
            //DblClient ??= new DblClient(_client.CurrentUser.Id, _config["token_dbl"]);

            if (DateTime.UtcNow - LastGuildCountUpdate < UpdateCooldown)
                return;

            //await BoatClient.UpdateGuildCountAsync((await _client.Rest.GetGuildsAsync()).Count).ConfigureAwait(false);
            //await DblClient.UpdateStatsAsync((await _client.Rest.GetGuildsAsync()).Count).ConfigureAwait(false);
            LastGuildCountUpdate = DateTime.UtcNow;
        }

        public async Task ReadInputAsync(SocketMessage arg)
        {
            // Ignore bots
            if (arg.Author.IsBot)
                return;

            // Ignore users if they are in a game session
            if (_games.ReservedUsers.ContainsKey(arg.Author.Id))
                return;

            // Ignore system messages
            if (!(arg is SocketUserMessage source))
                return;

            var ctx = new ArcadeContext(_client, _container, source);
            bool deleteInput = false;
            bool prefixFound = false;
            string input = null;
            // TODO: Handle message filters here to prevent additional command execution within another listener

            // Check all possible prefix formats
            int i = 0;

            if (source.HasMentionPrefix(_client.CurrentUser, ref i))
            {
                await ExecuteAsync(ctx, i);
                return;
            }

            if (source.HasStringPrefix(ctx.GetPrefix() + "d]", ref i))
            {
                prefixFound = true;
                deleteInput = true;
            }

            // Move this functionality to [about <input>
            if (source.HasStringPrefix(ctx.GetPrefix() + "?]", ref i))
            {
                string inner = source.Content[(ctx.GetPrefix() + "?]").Length..].ToLower();

                Console.WriteLine(inner);

                if (inner.EqualsAny("gimi"))
                {
                    await source.Channel.SendMessageAsync(CommandDetailsViewer.WriteGimi());

                    if (ctx.Account != null)
                        ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);

                    return;
                }

                if (inner.EqualsAny("doubler", "double", "dbl"))
                {
                    await source.Channel.SendMessageAsync(CommandDetailsViewer.WriteTick());

                    if (ctx.Account != null)
                        ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);

                    return;
                }

                if (inner == "getchips")
                {
                    await source.Channel.SendMessageAsync(CommandDetailsViewer.WriteGetChips());

                    if (ctx.Account != null)
                        ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);

                    return;
                }

                input = inner.StartsWith("help") ? inner.Equals("help") ? $"help {inner}" : inner : $"help {inner}";
                prefixFound = true;
            }

            if (!prefixFound)
            {
                if (source.HasStringPrefix(ctx.GetPrefix(), ref i))
                {
                    prefixFound = true;
                }
            }

            if (prefixFound)
            {
                await ExecuteAsync(ctx, i, input);

                if (deleteInput)
                    await ctx.Message.TryDeleteAsync();
            }
        }

        private static string WriteCooldownText(DateTime expiry, bool isGlobal = true)
        {
            var text = new StringBuilder();

            text.AppendLine($"> 🔻 {(isGlobal ? "You are executing commands too quickly." : "This command is on cooldown.")}");
            text.AppendLine($"> {(isGlobal ? "Please wait" : "You can execute it in")} {Format.LongCounter(DateTime.UtcNow - expiry)}.");

            return text.ToString();
        }

        // This finds the exact same command that will be executed
        // There is no double that this is an extensive operation
        // Try to simplify this system
        private async Task<CommandInfo> FindBestCommand(ArcadeContext ctx, int argPos)
        {
            SearchResult searchResult = _service.Search(ctx, argPos);

            var preconditionResults = new Dictionary<CommandMatch, PreconditionResult>();
            if (searchResult.IsSuccess)
            {
                foreach (CommandMatch match in searchResult.Commands)
                {
                    preconditionResults[match] = await match.Command.CheckPreconditionsAsync(ctx, _provider);
                }

                KeyValuePair<CommandMatch, PreconditionResult>[] successfulPreconditions = preconditionResults
                    .Where(x => x.Value.IsSuccess)
                    .ToArray();

                if (successfulPreconditions.Length > 0)
                {
                    var parseResultsDict = new Dictionary<CommandMatch, ParseResult>();

                    foreach ((CommandMatch key, PreconditionResult value) in successfulPreconditions)
                    {
                        ParseResult parseResult = await key.ParseAsync(ctx, searchResult, value, _provider).ConfigureAwait(false);
                        parseResultsDict[key] = parseResult;
                    }

                    float CalculateScore(CommandMatch match, ParseResult parseResult)
                    {
                        float argValuesScore = 0, paramValuesScore = 0;

                        if (match.Command.Parameters.Count > 0)
                        {
                            float argValuesSum = parseResult.ArgValues?
                                .Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
                            float paramValuesSum = parseResult.ParamValues?
                                .Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

                            argValuesScore = argValuesSum / match.Command.Parameters.Count;
                            paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
                        }

                        float totalArgsScore = (argValuesScore + paramValuesScore) / 2;
                        return match.Command.Priority + totalArgsScore * 0.99f;
                    }

                    var parseResults = parseResultsDict.OrderByDescending(x => CalculateScore(x.Key, x.Value));

                    var successfulParses = parseResults
                        .Where(x => x.Value.IsSuccess)
                        .ToArray();

                    if (successfulParses.Length > 0)
                    {
                        var chosenOverload = successfulParses[0];
                        return chosenOverload.Key.Command;
                    }
                }
            }

            return null;
        }

        public async Task ExecuteAsync(ArcadeContext ctx, int argPos, string input = null)
        {
            if (ctx.Account != null)
            {
                if (ctx.Account.GlobalCooldown.HasValue)
                {
                    if (!(DateTime.UtcNow - ctx.Account.GlobalCooldown >= TimeSpan.Zero))
                    {
                        if ((ctx.Server?.Config.AllowCooldownNotices ?? true) && !ctx.Account.HasBeenNoticed)
                        {
                            await ctx.Channel.SendMessageAsync(WriteCooldownText(ctx.Account.GlobalCooldown.Value));
                            ctx.Account.HasBeenNoticed = true;
                        }

                        return;
                    }

                    ctx.Account.HasBeenNoticed = false;
                }

                if (ctx.Account.InternalCooldowns.Count > 0)
                {
                    CommandInfo possibleCommand = await FindBestCommand(ctx, argPos);

                    if (possibleCommand != null)
                    {
                        string id = ContextNode.GetId(possibleCommand);
                        Console.WriteLine(id);

                        if (ctx.Account.InternalCooldowns.ContainsKey(id))
                        {
                            if (!(DateTime.UtcNow - ctx.Account.InternalCooldowns[id] >= TimeSpan.Zero))
                            {
                                await ctx.Channel.SendMessageAsync(WriteCooldownText(ctx.Account.InternalCooldowns[id], false));
                                ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);
                                return;
                            }
                        }

                        if ((possibleCommand.Attributes.FirstOrDefault<SessionAttribute>() != null || possibleCommand.Attributes.FirstOrDefault<RequireNoSessionAttribute>() != null)
                            && ctx.Account.IsInSession)
                        {
                            await ctx.Channel.SendMessageAsync(Format.Warning("You are currently in a session."));
                            ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);
                            return;
                        }

                        if (possibleCommand.Attributes.FirstOrDefault<SessionAttribute>() != null)
                        {
                            ctx.Account.IsInSession = true;
                        }
                    }
                }
            }

            if (ctx.Account != null)
                ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(GlobalCooldown);

            if (!string.IsNullOrWhiteSpace(input))
                await _service.ExecuteAsync(ctx, input, _provider);
            else
                await _service.ExecuteAsync(ctx, argPos, _provider);
        }

        private async Task OnExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // TODO: Make the specific context exchangeable.

            if (!(context is ArcadeContext ctx))
                throw new Exception("Invalid context provided.");

            // Remove any session locks IF the command that was executed invoked a session call
            if (command.IsSpecified && command.Value.Attributes.FirstOrDefault<SessionAttribute>() != null && ctx.Account != null)
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
                    // Ignore unknown commands
                    if (result.Error == CommandError.UnknownCommand)
                        return;

                    if (result.Error == CommandError.ObjectNotFound)
                    {
                        await context.Channel.SendMessageAsync($"> {Icons.Warning} **Odd.**\n> {result.ErrorReason ?? "An unknown error has occurred."}");
                        return;
                    }

                    await context.Channel.ThrowAsync(result.ErrorReason);
                }

                return;
            }

            // Otherwise
            if (command.IsSpecified)
            {
                // Handle all data
                await UpdateAsync(command.Value, ctx);
            }
        }

        // update all accounts and users accordingly based on the command
        private async Task UpdateAsync(CommandInfo command, ArcadeContext ctx)
        {
            var cooldown = command.Attributes.FirstOrDefault<CooldownAttribute>();

            // If the cooldown and account both exist
            if (cooldown != null && ctx.Account != null)
            {
                string id = ContextNode.GetId(command, true);

                if (!ctx.Account.InternalCooldowns.TryAdd(id, DateTime.UtcNow.Add(cooldown.Duration)))
                    ctx.Account.InternalCooldowns[id] = DateTime.UtcNow.Add(cooldown.Duration);
            }

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
            else if (!JsonHandler.JsonExists<ArcadeUser>(ctx.User.Id))
            {
                if (ctx.Account == null)
                    ctx.GetOrAddUser(ctx.User);

                ctx.Data.Users.TrySave(ctx.Account);
            }

            // Check if the guild was updated or doesn't exist to save
            var requireGuild = command.Preconditions.FirstOrDefault<RequireGuildAttribute>();

            if (requireGuild?.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly) ?? false)
            {
                ctx.Data.Guilds.TrySave(ctx.Server);
            }
            else if (!JsonHandler.JsonExists<BaseGuild>(ctx.Guild.Id))
            {
                if (ctx.Guild == null)
                    ctx.GetOrAddGuild(ctx.Guild);

                ctx.Data.Guilds.TrySave(ctx.Server);
            }

            var requireGlobal = command.Attributes.FirstOrDefault<RequireDataAttribute>();

            if (requireGlobal != null)
                ctx.Data.SaveGlobalData();
        }
    }
}
