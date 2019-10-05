using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo
{

    // this is used to store specific phases
    public class GameTask
    {
        // required to handle message input.
        private BaseSocketClient _client;
        // name: The task ID, attributes: A list of attributes this task keeps track of. triggers: a list of triggers that update within the task.
        internal GameTask(BaseSocketClient client, string id, List<GameAttribute> attributes, List<GameTrigger> triggers, List<GameRoute> routes, List<GameSuccessRoute> successRoutes, TimeSpan? timeout = null)
        {
            Id = id;
            Cancel = new TaskCompletionSource<bool>();
            Timeout = null;
        }
        public string Id { get; }
        private TaskCompletionSource<bool> Cancel { get; }
        // a collection of attributes deriving from the main game handler.
        // private List<GameAttributes> _rootAttributes;
        private TaskCompletionSource<bool> Success { get; }
        private List<GameRoute> Routes { get; }
        private List<GameSuccessRoute> SuccessRoutes { get; }

        // a collection of triggers that are read when a message is sent.
        internal List<GameTrigger> Triggers { get; }

        // a collection of local attributes.
        public List<GameAttribute> Attributes { get; }

        // starts
        public async Task StartAsync()
        {
            // this handles messages when sent.
            async Task ReadAsync(SocketMessage message)
            {

            };

            // set up the listener
            _client.MessageReceived += ReadAsync;
            List<Task> tasks = new List<Task>();
            tasks.Add(Cancel.Task); // when the task was cancelled.
            tasks.Add(Success.Task); // when the criteria has been met
            if (Timeout.HasValue)
                tasks.Add(Task.Delay(Timeout.Value)); // timer

            Task task = await Task.WhenAny(tasks).ConfigureAwait(false);
            // create a loop until either: all criteria have been met; the timeout has finished; the cancel token has gone off.
        }

        // the optional amount of time before the task is completed.
        public TimeSpan? Timeout { get; }

        public bool IsCompleted { get; } // this can derive from TaskCompletionSource<bool>

        public void End()
            => Cancel.SetResult(true);

        // this is a list of all tasks that this current task can transition to, based on the criteria.
        // Dictionary<string, GameCriteria> Transitions;
    }
}
