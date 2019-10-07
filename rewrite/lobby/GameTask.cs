using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    // this is used to store specific phases
    public class GameTask
    {
        internal GameTask(string id, List<GameAttribute> attributes, List<GameTrigger> triggers, List<TaskCriterion> criteria, GameRoute onCancel, GameTimer timer = null)
        {
            // set up the root basics. we need a root game handler that contains root attributes and so forth.
            Id = id;

            // these are the properties that control how the tasks functions.
            Attributes = attributes; // a list of attributes that are used to determine scenarios.
            Triggers = triggers; // a list of triggers used to update attributes
            Criteria = criteria; // a list of criteria used to determine what a successful route is.

            // create the task completion sources to read from.
            Success = new TaskCompletionSource<GameRoute>();
            OnCancel = onCancel;
            Cancel = new TaskCompletionSource<GameRoute>();
            Timer = timer; // a timer used to control how long the task lasts until it closes.
        }
        public string Id { get; }
        private TaskCompletionSource<GameRoute> Cancel { get; }
        private GameRoute OnCancel { get; }
        private TaskCompletionSource<GameRoute> Success { get; }
        private GameTimer Timer { get; }
        private List<TaskCriterion> Criteria { get; }
        internal List<GameTrigger> Triggers { get; }

        // a collection of local attributes.
        public List<GameAttribute> Attributes { get; }
        private GameAttribute LastAttributeUpdated { get; set; } = null;

        public async Task<GameRoute> StartAsync(BaseSocketClient client, GameLobby lobby, GameEventHandler events, GameData data, CancellationToken token)
        {
            async Task ParseAsync(SocketMessage message)
            {
                LastAttributeUpdated = null;

                // if this message came from a receiver location AND the receiver is currently active for a game:
                if (!(lobby.Receivers.Any(x => x.ChannelId == message.Channel.Id && x.State == GameState.Active) && lobby.Users.Any(x => x.Id == message.Author.Id)))
                    return;

                foreach (GameTrigger trigger in Triggers)
                {
                    if (trigger.TryParse(message.Content, lobby.Users, out TriggerContext context))
                    {
                        if (context.AttributeUpdate != null)
                            if (UpdateAttribute(context.AttributeUpdate, out GameAttribute attribute))
                                LastAttributeUpdated = attribute;
                        break;
                    }
                }

                if (LastAttributeUpdated != null)
                    foreach (TaskCriterion criteria in Criteria.Where(x => x.Requirements.Any(y => y.RequiredId == LastAttributeUpdated.Id)))
                        if (criteria.Check(Attributes))
                            Success.SetResult(criteria.OnSuccess);
            }

            try
            {
                return await Task.Run(async () =>
                {
                    client.MessageReceived += ParseAsync;
                    List<Task<GameRoute>> tasks = new List<Task<GameRoute>> { Cancel.Task, Success.Task };

                    if (Timer != null)
                        tasks.Add(Timer.Run());

                    Task<GameRoute> task = await Task.WhenAny(tasks).ConfigureAwait(false);

                    task.Result.LastTaskId = Id;
                    client.MessageReceived -= ParseAsync;

                    return task.Result;
                }, token);
            }
            catch (OperationCanceledException)
            {
                client.MessageReceived -= ParseAsync;
                Console.WriteLine("Cancel requested.");
                return GameRoute.Empty;
            }
        }

        private bool UpdateAttribute(GameAttributeUpdate update, out GameAttribute attribute)
        {
            if (!Attributes.Any(x => x.Id == update.Id))
                throw new Exception("The update packet is trying to update an attribute that doesn't exist.");
            attribute = Attributes.First(x => x.Id == update.Id);
            attribute.Value += update.Amount; // consider Append/Set method types.
            return true;
        }
        public void End()
            => Cancel.SetResult(OnCancel);
    }
}
