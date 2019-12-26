using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Orikivo
{

    public class MessageCollector
    {
        private readonly BaseSocketClient _client;

        public MessageCollector(BaseSocketClient client)
        {
            _client = client;
        }

        public TimeSpan? ElapsedTime { get; private set; }

        public async Task MatchAsync(MessageFilter filter, MatchOptions options = null)
            => await MatchAsync(filter.Judge, options);
        public async Task MatchAsync(Func<SocketMessage, int, bool> filter, MatchOptions options = null)
        {
            options ??= MatchOptions.Default;
            FilterMatch match = null;
            AsyncTimer timer = new AsyncTimer(options.Timeout);
            TaskCompletionSource<bool> complete = new TaskCompletionSource<bool>();

            await options.Action.OnStartAsync();

            int index = 0;
            async Task HandleAsync(SocketMessage arg)
            {
                bool filterSuccess = filter.Invoke(arg, index);
                match = new FilterMatch(arg, index, filterSuccess);

                if (filterSuccess)
                {
                    ActionResult result = (await options.Action.InvokeAsync(arg));

                    switch (result)
                    {
                        case ActionResult.Fail:
                            complete.SetResult(false);
                            break;

                        case ActionResult.Success:
                            complete.SetResult(true);
                            break;

                        case ActionResult.Continue:
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
                await options.Action.OnTimeoutAsync(match?.Message);

            Console.WriteLine("Match handled.");
        }

        public async Task<FilterMatch> TryFilterAsync(MessageFilter filter, FilterOptions options = null)
            => await TryFilterAsync(filter.Judge, options);
        public async Task<FilterMatch> TryFilterAsync(Func<SocketMessage, int, bool> filter, FilterOptions options = null)
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

        public async Task<FilterCollection> CollectAsync(MessageFilter filter, CollectionOptions options = null)
            => await CollectAsync(filter.JudgeMany, options);
        public async Task<FilterCollection> CollectAsync(Func<SocketMessage, FilterCollection, int, bool> filter, CollectionOptions options = null)
        {
            options ??= CollectionOptions.Default;
            FilterCollection matches = new FilterCollection();

            AsyncTimer timer = new AsyncTimer(options.Timeout);

            TaskCompletionSource<bool> complete = new TaskCompletionSource<bool>();

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

                if (options.Capacity.HasValue)
                    if (matches.Count == options.Capacity.Value)
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
