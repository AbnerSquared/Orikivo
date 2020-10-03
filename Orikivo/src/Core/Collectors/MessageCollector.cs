using Discord.WebSocket;
using System;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Orikivo
{
    /// <summary>
    /// Represents a handler for collecting inbound messages on <see cref="Discord"/>.
    /// </summary>
    public class MessageCollector
    {
        private readonly BaseSocketClient _client;

        /// <summary>
        /// Initializes a new <see cref="MessageCollector"/> with the specified <see cref="BaseSocketClient"/>.
        /// </summary>
        /// <param name="client">The <see cref="BaseSocketClient"/> that will be referenced to read inbound messages.</param>
        public MessageCollector(BaseSocketClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Represents the amount of time that passed during a handled process (only updated at the end of each handling).
        /// </summary>
        public TimeSpan? ElapsedTime { get; private set; }

        /// <summary>
        /// Starts a message handler session for this <see cref="MessageCollector"/>.
        /// </summary>
        /// <param name="filter">The filter that will be used to compare messages.</param>
        /// <param name="options">The options that will be used to set up the <see cref="MessageCollector"/>.</param>
        public async Task MatchAsync(MessageFilter filter, SessionOptions options = null)
            => await MatchAsync(filter.Judge, options);

        /// <summary>
        /// Starts a message handler session for this <see cref="MessageCollector"/>.
        /// </summary>
        /// <param name="filter">The filter that will be used to compare messages.</param>
        /// <param name="options">The options that will be used to set up the <see cref="MessageCollector"/>.</param>
        public async Task MatchAsync(FilterDelegate filter, SessionOptions options = null)
        {
            options ??= SessionOptions.Default;
            FilterMatch match = null;
            var timer = new AsyncTimer(options.Timeout);
            var complete = new TaskCompletionSource<bool>();

            await options.Session.OnStartAsync();

            int index = 0;
            async Task HandleAsync(SocketMessage arg)
            {
                bool filterSuccess = filter.Invoke(arg, index);
                match = new FilterMatch(arg, index, filterSuccess);

                if (filterSuccess)
                {
                    MatchResult result = await options.Session.InvokeAsync(arg);

                    switch (result)
                    {
                        case MatchResult.Fail:
                            complete.SetResult(false);
                            break;

                        case MatchResult.Success:
                            complete.SetResult(true);
                            break;

                        case MatchResult.Continue:
                        default:
                            break;
                    }
                }

                if (options.ResetTimeoutOnAttempt)
                    timer.Reset();

                index++;
            }

            _client.MessageReceived += HandleAsync;

            if (options.Timeout.HasValue)
                timer.Start();

            Task<bool> possible = await Task.WhenAny(timer.CompletionSource.Task, complete.Task);


            _client.MessageReceived -= HandleAsync;
            ElapsedTime = timer.ElapsedTime;

            if (timer.Elapsed)
                await options.Session.OnTimeoutAsync(match?.Message);

            Console.WriteLine("Match handled.");
        }

        /// <summary>
        /// Tells the <see cref="MessageCollector"/> to attempt to match a single message.
        /// </summary>
        /// <param name="filter">The filter that will be used to compare messages.</param>
        /// <param name="options">The options that will be used to set up the <see cref="MessageCollector"/>.</param>
        public async Task<FilterMatch> TryFilterAsync(MessageFilter filter, FilterOptions options = null)
            => await TryFilterAsync(filter.Judge, options);

        /// <summary>
        /// Tells the <see cref="MessageCollector"/> to attempt to match a single message.
        /// </summary>
        /// <param name="filter">The raw filter that will be used to compare messages.</param>
        /// <param name="options">The options that will be used to set up the <see cref="MessageCollector"/>.</param>
        public async Task<FilterMatch> TryFilterAsync(FilterDelegate filter, FilterOptions options = null)
        {
            options ??= FilterOptions.Default;
            FilterMatch match = null;

            AsyncTimer timer = new AsyncTimer(options.Timeout);
            TaskCompletionSource<bool> complete = new TaskCompletionSource<bool>();

            int attempts = 0;
            async Task HandleAsync(SocketMessage arg)
            {
                bool isSuccess = filter.Invoke(arg, attempts);
                match = new FilterMatch(arg, attempts, isSuccess);

                if (isSuccess)
                {
                    complete.SetResult(true);
                }
                else if (options.MaxAttempts.HasValue)
                {
                    if (attempts == options.MaxAttempts.Value)
                        complete.SetResult(false);
                }

                if (options.ResetTimeoutOnAttempt)
                    timer.Reset();

                attempts++;
            }

            _client.MessageReceived += HandleAsync;

            if (options.Timeout.HasValue)
                timer.Start();

            await Task.WhenAny(timer.CompletionSource.Task, complete.Task);

            _client.MessageReceived -= HandleAsync;
            ElapsedTime = timer.ElapsedTime;

            return match;
        }

        /// <summary>
        /// Tells the <see cref="MessageCollector"/> to begin collecting messages.
        /// </summary>
        /// <param name="filter">The filter that will be used when comparing messages.</param>
        /// <param name="options">The options that will be used to set up the <see cref="MessageCollector"/>.</param>
        public async Task<FilterCollection> CollectAsync(MessageFilter filter, CollectionOptions options = null)
            => await CollectAsync(filter.JudgeMany, options);

        /// <summary>
        /// Tells the <see cref="MessageCollector"/> to begin collecting messages.
        /// </summary>
        /// <param name="filter">The raw filter that will be used when comparing messages.</param>
        /// <param name="options">The options that will be used to set up the <see cref="MessageCollector"/>.</param>
        public async Task<FilterCollection> CollectAsync(FilterCollectionDelegate filter, CollectionOptions options = null)
        {
            options ??= CollectionOptions.Default;
            var matches = new FilterCollection();
            var timer = new AsyncTimer(options.Timeout);
            var complete = new TaskCompletionSource<bool>();

            int index = 0;
            async Task HandleAsync(SocketMessage arg)
            {
                bool isSuccess = filter.Invoke(arg, matches, index);

                if (options.IncludeFailedMatches)
                    matches.Add(new FilterMatch(arg, index, isSuccess));
                else if (isSuccess)
                    matches.Add(new FilterMatch(arg, index, isSuccess));

                if (isSuccess && options.ResetTimeoutOnMatch)
                    timer.Reset();

                if (options.Capacity.HasValue && matches.Count == options.Capacity.Value)
                {
                    timer.Stop();
                    complete.SetResult(true);
                }

                index++;
            }

            _client.MessageReceived += HandleAsync;

            if (options.Timeout.HasValue)
                timer.Start();

            await Task.WhenAny(timer.CompletionSource.Task, complete.Task);

            _client.MessageReceived -= HandleAsync;

            ElapsedTime = timer.ElapsedTime;
            return matches;
        }
    }
}
