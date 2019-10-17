using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    // Defines a continuous action for a game that is called until any completion criteria have been met
    public class GameTask
    {
        // this is used to store specific phases
        internal GameTask(string id, List<GameAttribute> attributes, List<GameTrigger> triggers, List<TaskCriterion> criteria, GameTaskQueue onCancel, GameTimer timer = null)
        {
            // set up the root basics. we need a root game handler that contains root attributes and so forth.
            Id = id;

            // these are the properties that control how the tasks functions.
            Attributes = attributes; // a list of attributes that are used to determine scenarios.
            Triggers = triggers; // a list of triggers used to update attributes
            Criteria = criteria; // a list of criteria used to determine what a successful route is.

            // create the task completion sources to read from.
            Success = new TaskCompletionSource<GameTaskQueue>();
            OnCancel = onCancel;
            Cancel = new TaskCompletionSource<GameTaskQueue>();
            Timer = timer; // a timer used to control how long the task lasts until it closes.
        }
        public string Id { get; }
        private TaskCompletionSource<GameTaskQueue> Cancel { get; }
        private GameTaskQueue OnCancel { get; }
        private TaskCompletionSource<GameTaskQueue> Success { get; }
        private GameTimer Timer { get; }
        private List<TaskCriterion> Criteria { get; }
        internal List<GameTrigger> Triggers { get; }

        // a collection of local attributes.
        public List<GameAttribute> Attributes { get; }
        private List<GameAttribute> LastAttributesUpdated { get; set; } = new List<GameAttribute>();

        public async Task<GameTaskQueue> StartAsync(BaseSocketClient client, GameLobby lobby, GameMonitor monitor, GameData data, CancellationToken token)
        {
            Console.WriteLine($"-- Now starting task. ({Id}) --");
            await monitor.UpdateDisplayAsync(GameState.Active, $"[Console] Task opened. ({Id})");
            async Task ParseAsync(SocketMessage message)
            {
                StringBuilder content = new StringBuilder();
                LastAttributesUpdated = new List<GameAttribute>();

                Console.WriteLine($"-- Now validating incoming message... --");
                // if this message came from a receiver location AND the receiver is currently active for a game:
                if (!(lobby.Receivers.Any(x => x.ChannelId == message.Channel.Id && x.State == GameState.Active) && lobby.TryGetUser(message.Author.Id, out User user)))
                {
                    Console.WriteLine($"-- Invalid message. The user was not in the lobby or the receiver it came from is on a different channel. --");
                    return;
                }
                Console.WriteLine($"-- Message validated. --");

                foreach (GameTrigger trigger in Triggers)
                {
                    Console.WriteLine($"-- Now comparing a trigger. (trigger:{trigger.Name}) --");
                    if (trigger.TryParse(message.Content, lobby.Users, out TriggerResult context))
                    {
                        content.AppendLine($"[Console] Trigger successful. (trigger:{trigger.Name})");
                        if (context.Packets != null)
                            foreach (AttributeUpdatePacket packet in context.Packets)
                                if (UpdateAttribute(packet, out GameAttribute attribute))
                                    LastAttributesUpdated.Add(attribute);
                        break;
                    }
                }

                if (LastAttributesUpdated.Count == 0)
                    content.AppendLine($"[{user.Name}]: {message.Content}");

                if (LastAttributesUpdated.Count > 0)
                    foreach (TaskCriterion criteria in Criteria.Where(x => x.AttributeCriteria.Any(y => LastAttributesUpdated.Any(z => z.Id == y.RequiredId))))
                        if (criteria.Check(Attributes))
                        {
                            content.AppendLine($"[Console] Task closed. ({Id} => {criteria.OnSuccess.TaskId})");
                            Success.SetResult(criteria.OnSuccess);
                            break;
                        }

                // this now only calls up once per trigger update.
                await monitor.UpdateDisplayAsync(GameState.Active, content.ToString().Trim());
            }
            //try
            //{
                //return await Task.Run(async () =>
                //{
            client.MessageReceived += ParseAsync;
            List<Task<GameTaskQueue>> tasks = new List<Task<GameTaskQueue>> { Cancel.Task, Success.Task };

            if (Timer != null)
                tasks.Add(Timer.Run());

            Task<GameTaskQueue> task = await Task.WhenAny(tasks).ConfigureAwait(false);

            task.Result.LastTaskId = Id;
            client.MessageReceived -= ParseAsync;
            Console.WriteLine($"-- A task has ended. (task:{Id}) --");
            return task.Result;
                //}, token);
            //}
            //catch (OperationCanceledException)
            //{
            //    _client.MessageReceived -= ParseAsync;
            //    Console.WriteLine("Cancel requested.");
            //    return GameRoute.Empty;
            //}
        }

        private bool UpdateAttribute(AttributeUpdatePacket update, out GameAttribute attribute)
        {
            if (!Attributes.Any(x => x.Id == update.Id))
                throw new Exception("The update packet is trying to update an attribute that doesn't exist.");
            attribute = Attributes.First(x => x.Id == update.Id);
            attribute.Value += update.Amount;

            // method used here.
            switch(update.Method)
            {
                case AttributeUpdateMethod.Add:
                    attribute.Value += update.Amount;
                    break;
                case AttributeUpdateMethod.Set:
                    attribute.Value = update.Amount;
                    break;
                case AttributeUpdateMethod.Remove:
                    attribute.Value -= update.Amount;
                    break;
            }

            Console.WriteLine($"-- An attribute was updated. ({attribute.Id}, {attribute.Value - update.Amount} => {attribute.Value}) --");
            return true;
        }

        public void End()
            => Cancel.SetResult(OnCancel);
    }
}
