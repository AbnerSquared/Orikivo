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
        /// Starts an asynchronous <see cref="MessageSession"/> for this <see cref="MessageCollector"/>.
        /// </summary>
        /// <param name="session">The <see cref="MessageSession"/> that will be used for this <see cref="MessageCollector"/>.</param>
        /// <param name="filter">The filter that will be used to compare messages.</param>
        /// <param name="options">The options that will be used to set up the <see cref="MessageCollector"/>.</param>
        public async Task RunSessionAsync(MessageSession session, FilterDelegate filter, SessionOptions options = null)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session), "The specified MessageSession cannot be null");

            options ??= SessionOptions.Default;
            MessageMatch match = null;
            var timer = new AsyncTimer(options.Timeout);
            var complete = new TaskCompletionSource<bool>();

            await session.OnStartAsync();

            int index = 0;
            async Task HandleAsync(SocketMessage arg)
            {
                bool filterSuccess = filter.Invoke(arg, index);
                match = new MessageMatch(arg, index, filterSuccess);

                if (filterSuccess)
                {
                    SessionTaskResult result = await session.OnMessageReceivedAsync(arg);

                    switch (result)
                    {
                        case SessionTaskResult.Fail:
                            complete.SetResult(false);
                            break;

                        case SessionTaskResult.Success:
                            complete.SetResult(true);
                            break;

                        case SessionTaskResult.Continue:
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

            await Task.WhenAny(timer.CompletionSource.Task, complete.Task);

            _client.MessageReceived -= HandleAsync;
            ElapsedTime = timer.ElapsedTime;

            if (timer.Elapsed)
                await session.OnTimeoutAsync(match?.Message);

            Console.WriteLine("Match handled.");
        }

        /// <summary>
        /// Tells the <see cref="MessageCollector"/> to asynchronously attempt to match a single message.
        /// </summary>
        /// <param name="filter">The raw filter that will be used to compare messages.</param>
        /// <param name="options">The options that will be used to set up the <see cref="MessageCollector"/>.</param>
        public async Task<MessageMatch> MatchSingleAsync(FilterDelegate filter, MatchOptions options = null)
        {
            options ??= MatchOptions.Default;
            MessageMatch match = null;

            var timer = new AsyncTimer(options.Timeout);
            var complete = new TaskCompletionSource<bool>();

            int attempts = 0;
            async Task HandleAsync(SocketMessage arg)
            {
                bool isSuccess = filter.Invoke(arg, attempts);
                match = new MessageMatch(arg, attempts, isSuccess);

                if (isSuccess)
                {
                    complete.SetResult(true);
                }
                else if (options.MaxAttempts.HasValue && attempts == options.MaxAttempts.Value)
                {
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
        /// Tells the <see cref="MessageCollector"/> to begin collecting messages asynchronously.
        /// </summary>
        /// <param name="filter">The raw filter that will be used when comparing messages.</param>
        /// <param name="options">The options that will be used to set up the <see cref="MessageCollector"/>.</param>
        public async Task<MessageMatchCollection> CollectAsync(FilterCollectionDelegate filter, CollectionOptions options = null)
        {
            options ??= CollectionOptions.Default;
            var matches = new MessageMatchCollection();
            var timer = new AsyncTimer(options.Timeout);
            var complete = new TaskCompletionSource<bool>();

            int index = 0;
            async Task HandleAsync(SocketMessage arg)
            {
                bool isSuccess = filter.Invoke(arg, matches, index);

                if (isSuccess || options.IncludeFailedMatches)
                    matches.Add(new MessageMatch(arg, index, isSuccess));

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
