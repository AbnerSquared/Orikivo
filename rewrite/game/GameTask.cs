using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo
{
    // TODO: Create private message listeners for specific users.
    // TODO: Handle display updates from a game packet.
    // CONCEPT: Allow for sub-tasks within a task?
    /// <summary>
    /// A task that loops until a criteria mentioned has been met.
    /// </summary>

    public class GameTask
    {
        // TODO: Build derived from game task properties.
        internal GameTask(GameTaskProperties properties)
        {

        }


        // this is used to store specific phases
        internal GameTask(string name, List<GameAttribute> attributes, List<GameTrigger> triggers, List<TaskCriterion> criteria, TaskQueuePacket onCancel, GameTimer timer = null)
        {
            // set up the root basics. we need a root game handler that contains root attributes and so forth.
            Name = name;

            // these are the properties that control how the tasks functions.
            Attributes = attributes; // a list of attributes that are used to determine scenarios.
            Triggers = triggers; // a list of triggers used to update attributes
            Criteria = criteria; // a list of criteria used to determine what a successful route is.

            OnCancel = onCancel;
            Timer = timer; // a timer used to control how long the task lasts until it closes.
        }

        public string Name { get; }
        public string Id => $"task.{Name}";
        public GameTaskData Data { get; }

        private TaskCompletionSource<TaskQueuePacket> Cancel = new TaskCompletionSource<TaskQueuePacket>();
        private TaskQueuePacket OnCancel { get; }
        private TaskCompletionSource<TaskQueuePacket> Success = new TaskCompletionSource<TaskQueuePacket>();
        private GameTimer Timer { get; }
        private List<TaskCriterion> Criteria { get; }
        internal List<GameTrigger> Triggers { get; }

        // a collection of local attributes.
        public List<GameAttribute> Attributes { get; }
        private List<GameAttribute> LastAttributesUpdated { get; set; } = new List<GameAttribute>();

        // TODO: It could be possible to make the game client function independently from Discord.
        public async Task<TaskQueuePacket> StartAsync(BaseSocketClient client, GameLobby lobby, GameDisplay display, GameData data, CancellationToken token)
        {
            Data.Root = data; // unite parent data to task data.
            Console.WriteLine($"-- Now starting task. ({Id}) --");
            display.UpdateWindow(GameState.Active, new ElementUpdatePacket(new Element($"[Console] Task opened. ({Id})"), ElementUpdateMethod.AddToGroup, groupId: "elements.chat"));
            async Task ParseAsync(SocketMessage message)
            {
                LastAttributesUpdated = new List<GameAttribute>();

                Console.WriteLine($"-- Now validating incoming message... --");
                // if this message came from a receiver location AND the receiver is currently active for a game:
                if (!(lobby.Receivers.Any(x => x.ChannelId == message.Channel.Id && x.State == GameState.Active) && lobby.TryGetUser(message.Author.Id, out User user)))
                {
                    Console.WriteLine($"-- Invalid message. The user was not in the lobby or the receiver it came from is on a different channel. --");
                    return;
                }
                Console.WriteLine($"-- Message validated. --");

                Player player = GetPlayer(message.Author.Id);

                foreach (GameTrigger trigger in Triggers)
                {
                    Console.WriteLine($"-- Now comparing a trigger. (trigger:{trigger.Name}) --");
                    if (trigger.TryParse(new TaskTriggerContext(Data, player, message.Content), out GameTriggerResult context))
                    {
                        display.UpdateWindow(GameState.Active, new ElementUpdatePacket(new Element($"[Console] Trigger successful. (trigger:{trigger.Name})"), ElementUpdateMethod.AddToGroup, groupId: "elements.chat"));
                        if (context.Packets != null)
                            foreach (GameUpdatePacket packet in context.Packets)
                                if (Update(packet, display, out List<GameAttribute> updatedAttributes))
                                    LastAttributesUpdated.AddRange(updatedAttributes);
                        break;
                    }
                }

                if (LastAttributesUpdated.Count == 0)
                    display.UpdateWindow(GameState.Active, new ElementUpdatePacket(new Element($"[{user.Name}]: {message.Content}"), ElementUpdateMethod.AddToGroup, groupId: "elements.chat"));

                if (LastAttributesUpdated.Count > 0)
                    foreach (TaskCriterion criteria in Criteria.Where(x =>
                    x.AttributeCriteria.Any(y => LastAttributesUpdated.Select(z => z.Id).Contains(y.RequiredId))))
                        if (criteria.Check(Attributes))
                        {
                            display.UpdateWindow(GameState.Active, new ElementUpdatePacket(new Element($"[Console] Task closed. ({Id} => {criteria.OnSuccess.NextTaskId})"), ElementUpdateMethod.AddToGroup, groupId: "elements.chat"));
                            Success.SetResult(criteria.OnSuccess);
                            break;
                        }

                // refresh all connected displays with its new content, if applicable.
                await display.RefreshAsync();
            }
            /*
             try
             {
                return await Task.Run(async () =>
                {
            */
            client.MessageReceived += ParseAsync;
            List<Task<TaskQueuePacket>> tasks = new List<Task<TaskQueuePacket>> { Cancel.Task, Success.Task };

            if (Timer != null)
                tasks.Add(Timer.Run());

            Task<TaskQueuePacket> task = await Task.WhenAny(tasks).ConfigureAwait(false);

            task.Result.TaskId = Id;
            client.MessageReceived -= ParseAsync;
            Console.WriteLine($"-- A task has ended. (task:{Id}) --");
            return task.Result;
            /*
                }, token);
            }
            catch (OperationCanceledException)
            {
                _client.MessageReceived -= ParseAsync;
                Console.WriteLine("Cancel requested.");
                return GameRoute.Empty;
            }
            */
        }

        private Player GetPlayer(ulong id)
            => Data?.Players.FirstOrDefault(x => x.UserId == id);

        private Player GetPlayer(int index)
            => Data?.Players.FirstOrDefault(x => x.Index == index);

        private bool Update(GameUpdatePacket packet, GameDisplay display, out List<GameAttribute> updatedAttributes)
        {
            updatedAttributes = new List<GameAttribute>();
            foreach (AttributeUpdatePacket attributePacket in packet.AttributePackets)
            {
                if (!UpdateAttribute(attributePacket, out GameAttribute updatedAttribute))
                    return false;

                updatedAttributes.Add(updatedAttribute);   
            }

            foreach (WindowUpdatePacket windowPacket in packet.WindowPackets)
                if (!display.UpdateWindow(GameState.Active, windowPacket))
                    return false;

            return true;
        }

        private bool UpdateAttribute(AttributeUpdatePacket update, out GameAttribute attribute)
        {
            if (!Attributes.Any(x => x.Id == update.Id))
                throw new Exception("The update packet is trying to update an attribute that doesn't exist.");
            attribute = Attributes.First(x => x.Id == update.Id);

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
