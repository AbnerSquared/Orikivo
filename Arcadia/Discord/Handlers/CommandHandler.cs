using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orikivo;
using Orikivo.Desync;
using Orikivo.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arcadia.Multiplayer;
using Format = Orikivo.Format;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Arcadia
{
    // This needs to handle reading commands
    public static class CommandDetails
    {
        public static string WriteTick()
        {
            var info = new StringBuilder();
            info.AppendLine($"> 🎰 **Casino**");
            info.AppendLine("> Double Machine\n");

            info.AppendLine($"This machine offers high risk with high reward, allowing you to gain a massive amount of {Icons.Chips} **Chips** in only a single run.\n");
            info.AppendLine($"> **Step 1: Generation**");
            info.AppendLine("In this phase, the machine rolls a tick chance. As long as the generated tick is below the current **Chance**, the machine reduces the **Chance** by **1**%, and continues generating ticks. Once the generated tick fails, the machine is stopped, returning the **Actual**.");
            info.AppendLine("\n> **Step 2: Calculation**");
            info.AppendLine("Once the **Actual** is found, it compares it with your **Method** to see if your **Guess** was successful.");
            info.AppendLine("\nIf your **Method** is exact:\n- If the **Actual** is equal to the **Guess**, your result is marked as **Exact**, otherwise it's marked as a **Loss**.");
            info.AppendLine("\nOtherwise, if your **Method** is below:\n- If the **Actual** is greater than or equal to the **Guess**, your result is marked as a **Win** (**Exact** if the **Actual** is equal to the **Guess**).");
            info.AppendLine("\n> **Step 3: Reward**");
            info.AppendLine($"If you were given a **Loss**, this returns a reward of {Icons.Chips} **0**.");
            info.AppendLine($"\nOtherwise, if you were given a **Win** or **Exact**, you are given a reward of:\n- {Icons.Chips} `floor(Bet * (2 ^ Actual) * (Method == Exact ? 1.5 : 1))`");

            return info.ToString();
        }

        public static string WriteGetChips()
        {
            var info = new StringBuilder();
            info.AppendLine("> **Chip Conversion**");
            info.AppendLine($"> {Icons.Balance} **1** ≈ {Icons.Chips} **{MoneyConvert.ToChips(1)}**");
            info.AppendLine($"> {Icons.Balance} **10** ≈ {Icons.Chips} **{MoneyConvert.ToChips(10)}**");
            info.AppendLine($"> {Icons.Balance} **100** ≈ {Icons.Chips} **{MoneyConvert.ToChips(100)}**");
            info.AppendLine("\nSpecify an amount to convert your **Orite** into **Chips**, for use at the **Casino**.");
            return info.ToString();
        }

        public static string WriteGimi()
        {
            var info = new StringBuilder();
            info.AppendLine($"> 🎰 **Casino**");
            info.AppendLine("> Gimi\n");
            info.AppendLine($"This is a built-in machine that provides {Icons.Balance} **Orite** or {Icons.Debt} **Debt** at random.\n");

            info.AppendLine($"> **Step 1: Generation**");
            info.AppendLine("In this phase, a **Seed** is rolled based on your **Risk** (default is **50**%) to determine your win state. If the **Seed** that was rolled landed within the range specified by your **Win Direction** (default is **Above**), you are given a **Win**. Otherwise, your result is a **Loss**.");
            info.AppendLine($"\nThe **Seed** is then checked for a match in the collection of specified golden slots. If a match was successful, your result is set to **Gold**. If a match failed, your result is left alone unless your **Seed** is 1 off of the **Gold**, which sets your result to **Curse**.");
            info.AppendLine($"\n> **Step 2: Reward**");
            info.AppendLine($"If your result was **Gold**, you are given {Icons.Balance} **50** and a **Pocket Lawyer**.\nIf your result was **Curse**, you are given {Icons.Debt} **200**.\nOtherwise, you are given a random amount of {Icons.Balance} **Orite** ({Icons.Debt} **Debt** if **Loss**) based on your **Risk**.");
            return info.ToString();
        }

    }

    public class CommandHandler
    {
        public static readonly TimeSpan GlobalCooldown = TimeSpan.FromSeconds(2);

        // Use a shorter cooldown if the command notice cooldown was too large
        public static readonly TimeSpan CommandNoticeCooldown = TimeSpan.FromSeconds(0.75);

        private readonly CommandService _service;
        private readonly DiscordSocketClient _client;
        private readonly ArcadeContainer _container;
        private readonly IServiceProvider _provider;
        private readonly InfoService _info;
        private readonly GameManager _games;

        public CommandHandler(DiscordSocketClient client, CommandService service, ArcadeContainer container,
            IServiceProvider provider, InfoService info, GameManager games)
        {
            _client = client;
            _service = service;
            _container = container;
            _provider = provider;
            _client.MessageReceived += ReadInputAsync;
            _service.CommandExecuted += OnExecutedAsync;
            _info = info;
            _games = games;
        }

        public string GetPrefix(ArcadeContext ctx)
            => ctx.Account?.Config.Prefix ?? ctx.Server?.Config.Prefix ?? OriGlobal.DEFAULT_PREFIX;

        public async Task ReadInputAsync(SocketMessage arg)
        {
            // Ignore bots
            if (arg.Author.IsBot)
                return;

            // Ignore users if they are in a game session
            if (_games.ReservedUsers.ContainsKey(arg.Author.Id))
                return;

            // Set up initial values
            SocketUserMessage source = arg as SocketUserMessage;
            ArcadeContext ctx = new ArcadeContext(_client, _container, source);
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

            if (source.HasStringPrefix(GetPrefix(ctx) + "d]", ref i))
            {
                prefixFound = true;
                deleteInput = true;
            }

            if (source.HasStringPrefix(GetPrefix(ctx) + "?]", ref i))
            {
                string inner = source.Content[(GetPrefix(ctx) + "?]").Length..].ToLower();

                Console.WriteLine(inner);

                if (inner.EqualsAny("gimi"))
                {
                    await source.Channel.SendMessageAsync(CommandDetails.WriteGimi());

                    if (ctx.Account != null)
                        ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);

                    return;
                }

                if (inner.EqualsAny("doubler", "double", "dbl"))
                {
                    await source.Channel.SendMessageAsync(CommandDetails.WriteTick());

                    if (ctx.Account != null)
                        ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);

                    return;
                }

                if (inner == "getchips")
                {
                    await source.Channel.SendMessageAsync(CommandDetails.WriteGetChips());

                    if (ctx.Account != null)
                        ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);

                    return;
                }

                input = inner.StartsWith("help") ? inner.Equals("help") ? $"help {inner}" : inner : $"help {inner}";
                prefixFound = true;
            }

            if (!prefixFound)
            {
                if (source.HasStringPrefix(GetPrefix(ctx), ref i))
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

                var successfulPreconditions = preconditionResults
                    .Where(x => x.Value.IsSuccess)
                    .ToArray();

                if (successfulPreconditions.Length > 0)
                {
                    var parseResultsDict = new Dictionary<CommandMatch, ParseResult>();

                    foreach (var pair in successfulPreconditions)
                    {
                        var parseResult = await pair.Key.ParseAsync(ctx, searchResult, pair.Value, _provider).ConfigureAwait(false);

                        /*
                        if (parseResult.Error == CommandError.MultipleMatches)
                        {
                            IReadOnlyList<TypeReaderValue> argList, paramList;

                            argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                            paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                            parseResult = ParseResult.FromSuccess(argList, paramList);

                            break;
                        }*/

                        parseResultsDict[pair.Key] = parseResult;
                    }

                    float CalculateScore(CommandMatch match, ParseResult parseResult)
                    {
                        float argValuesScore = 0, paramValuesScore = 0;

                        if (match.Command.Parameters.Count > 0)
                        {
                            var argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
                            var paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

                            argValuesScore = argValuesSum / match.Command.Parameters.Count;
                            paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
                        }

                        var totalArgsScore = (argValuesScore + paramValuesScore) / 2;
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
                        if (!ctx.Account.HasBeenNoticed)
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
                                await ctx.Channel.SendMessageAsync(WriteCooldownText(ctx.Account.InternalCooldowns[id],
                                    false));
                                ctx.Account.GlobalCooldown = DateTime.UtcNow.Add(CommandNoticeCooldown);
                                return;
                            }


                        }
                    }
                }
            }

            // TODO: Handle option parsing for commands

            // TODO: It might be required to create a custom parser and execution service separate from CommandService in order to properly
            // allow specific parsing methods

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
            var ctx = context as ArcadeContext;

            // Attempt to set a global cooldown on the account that executed this command

            // If the command failed
            if (!result.IsSuccess)
            {
                if (result is ExecuteResult execute)
                {
                    if (!result.IsSuccess)
                        await context.Channel.CatchAsync((execute).Exception);
                }
                else
                {
                    // Ignore unknown commands
                    if (result.Error == CommandError.UnknownCommand)
                        return;

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


            if (requireUser?.Handling.EqualsAny(AccountHandling.ReadWrite, AccountHandling.WriteOnly) ?? false)
            {
                MeritHelper.TryGiveMerits(ctx.Account);

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
            // For now, just save global data until a workaround is found
            // JsonHandler.Save(ctx.Container.Global, "global.json");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RequireDataAttribute : Attribute
    {

    }
}
