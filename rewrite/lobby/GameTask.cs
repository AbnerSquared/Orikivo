using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    public class GameTimer
    {
        internal GameTimer(TimeSpan timeout, GameRoute onTimeout)
        {
            Timeout = timeout;
            OnTimeout = OnTimeout;
        }
        public TimeSpan Timeout { get; }
        private GameRoute OnTimeout { get; }

        public async Task<GameRoute> Run()
        {
            await Task.Delay(Timeout);
            return OnTimeout;
        }
    }
    // this is used to store specific phases
    public class GameTask
    {
        // required to handle message input.
        private GameEventHandler _eventHandler;
        private BaseSocketClient _client;
        private GameLobby _lobby;

        // a collection of attributes deriving from the main game handler.
        // private List<GameAttributes> _rootAttributes;

        // name: The task ID, attributes: A list of attributes this task keeps track of. triggers: a list of triggers that update within the task.
        internal GameTask(string id, List<GameAttribute> attributes,
            List<GameTrigger> triggers, List<TaskCriterion> criteria, GameRoute onCancel, GameTimer timer = null)
        {
            // set up the root basics. we need a root game handler that contains root attributes and so forth.
            Id = id;

            // these are the properties that control how the tasks functions.
            Attributes = new GameAttributes(attributes); // a list of attributes that are used to determine scenarios.
            Triggers = triggers; // a list of triggers used to update attributes
            Criteria = criteria; // a list of criteria used to determine what a successful route is.

            // create the task completion sources to read from.
            Success = new TaskCompletionSource<GameRoute>();
            OnCancel = onCancel;
            Cancel = new TaskCompletionSource<GameRoute>();
            Timer = timer; // a timer used to control how long the task lasts until it closes.
        }
        public string Id { get; }
        private GameRoute OnCancel { get; }
        private TaskCompletionSource<GameRoute> Cancel { get; }
        
        private TaskCompletionSource<GameRoute> Success { get; }
        private GameTimer Timer { get; }
        // this provides where to go on specific attributes being complete.
        private List<TaskCriterion> Criteria { get; }

        // a collection of triggers that are read when a message is sent.
        internal List<GameTrigger> Triggers { get; }

        // a collection of local attributes.
        public GameAttributes Attributes { get; }

        // returns the next task to transition to on complete.

        // create handles on user leaving, with the user being actively in the game.

        public async Task<GameRoute> StartAsync(BaseSocketClient client, GameLobby lobby, GameEventHandler eventHandler)
        {
            _client = client;
            _lobby = lobby;
            _eventHandler = eventHandler;
            GameAttribute updatedAttribute = null;
            // this handles messages when sent.
            async Task ReadAsync(SocketMessage message)
            {
                // clear the last updated attribute.
                updatedAttribute = null;

                // set basic check vars
                ulong channelId = message.Channel.Id;
                ulong userId = message.Author.Id;
                string content = message.Content;

                // if this message came from a receiver location AND the receiver is currently active for a game:
                if (!(_lobby.Receivers.Any(x => x.ChannelId == channelId && x.State == GameState.Active) && _lobby.Users.Any(x => x.Id == userId)))
                    return; // cancel the check.

                TriggerContext ctx = null;
                // otherwise, if the message sent is a valid trigger:
                if (!Triggers.Any(x => x.TryParse(content, _lobby.Users, out ctx)))
                    return;

                if (ctx.AttributeUpdate != null)
                {
                    if (Attributes.Update(ctx.AttributeUpdate))
                        updatedAttribute = Attributes.Attributes.First(x => x.Name == ctx.AttributeUpdate.Id);
                }

                // send an optional display update on success

                if (updatedAttribute != null)
                    foreach (TaskCriterion criteria in Criteria.Where(x => x.Criteria.Any(y => y.AttributeId == updatedAttribute.Name)))
                        if (criteria.Check(Attributes.Attributes))
                            Success.SetResult(criteria.OnSuccess); // send an optional display update on success
            };

            // set up the listener
            _client.MessageReceived += ReadAsync;
            List<Task<GameRoute>> tasks = new List<Task<GameRoute>>();
            tasks.Add(Cancel.Task); // when the task was cancelled.
            tasks.Add(Success.Task); // when the criteria has been met
            if (Timer != null)
                tasks.Add(Timer.Run()); // timer Task.Delay(Timeout.Value) // nake a check on a timeout. set the result to the 

            Task<GameRoute> task = await Task.WhenAny(tasks).ConfigureAwait(false);
            _client.MessageReceived -= ReadAsync;
            return task.Result; // returns the route to take. if the taskId is left empty. the task is assumed as finished.
            // create a loop until either: all criteria have been met; the timeout has finished; the cancel token has gone off.
        }

        public bool IsCompleted { get; } // this can derive from TaskCompletionSource<bool>

        public void End()
            => Cancel.SetResult(OnCancel);
    }
}
