using System;
using System.Collections.Generic;
using System.Linq;
using Orikivo;
using Orikivo.Drawing.Graphics2D;

namespace Arcadia.Multiplayer
{
    public class GameSession
    {
        internal readonly GameServer _server;
        internal readonly GameBase Game;

        // create a game session with the information provided
        public GameSession(GameServer server, GameBase info)
        {
            StartedAt = DateTime.UtcNow;
            _server = server;
            Game = info;
            Game.SetGameConfig(server);
            Options = server.Config.GameOptions;
            Players = info.OnBuildPlayers(server.Players);
            Criteria = info.OnBuildRules(Players);
            Actions = info.OnBuildActions(Players);

            // base game actions required
            Actions.Add(new GameAction
            {
                Id = "end",
                UpdateOnExecute = true,
                OnExecute = delegate (GameContext ctx)
                {
                    ctx.Session.State = SessionState.Finish;
                    _server.EndCurrentSession();
                }
            });

            Actions.Add(new GameAction
            {
                Id = "destroy",
                UpdateOnExecute = true,
                OnExecute = delegate (GameContext ctx)
                {
                    _server.DestroyCurrentSession();
                }
            });

            Properties = info.OnBuildProperties();

            _server.Broadcasts.AddRange(info.OnBuildBroadcasts(Players));

            ActivityDisplay = "playing a game";
        }

        public DateTime StartedAt { get; set; }

        // this is used to display where the game is currently at
        public string ActivityDisplay { get; set; }

        // this is used to handle the current state of a session
        public SessionState State { get; set; } = SessionState.Continue;

        // these are all of the currently active players
        public List<PlayerData> Players { get; internal set; }

        // this is the options, derived from a game server
        public List<GameOption> Options { get; internal set; }

        // these are all of the attributes that are set
        public List<GameProperty> Properties { get; internal set; }

        // These are all of the possible actions
        public List<GameAction> Actions { get; set; }

        // these are all of the possible rules
        public List<GameCriterion> Criteria { get; set; }

        internal List<ActionQueue> ActionQueue = new List<ActionQueue>();
        private string _currentQueuedAction;

        // If true, nobody is allowed to invoke a command
        public bool BlockInput { get; set; }

        // If the action queued doesn't matter too much, just perform a quick fire-and-forget
        internal void QueueAction(TimeSpan delay, string actionId)
        {
            var timer = new ActionQueue(delay, actionId, this);
            ActionQueue.Add(timer);
            _currentQueuedAction = timer.Id;
        }

        // If working with an action outside of the list of actions
        internal void QueueAction(TimeSpan delay, Action<GameContext> action, bool updateOnExecute = true)
        {

        }

        // If the action queue does matter, give it a unique id so that it's easier to reference
        internal void QueueAction(string id, TimeSpan delay, string actionId)
        {
            var timer = new ActionQueue(id, delay, actionId, this);
            ActionQueue.Add(timer);
            _currentQueuedAction = timer.Id;
        }

        internal void QueueAction(string id, TimeSpan delay, Action<GameContext> action, bool updateOnExecute = true)
        {

        }

        internal void CancelNewestInQueue()
        {
            if (ActionQueue.Count == 0)
                return;

            if (GetNewestInQueue() == null)
                return;

            // marks this timer as cancelled.
            GetNewestInQueue().Cancel();

            ActionQueue.Remove(GetNewestInQueue());
        }

        internal void CancelInQueue(string id)
        {
            if (ActionQueue.Count == 0)
                return;

            if (GetInQueue(id) == null)
                return;
        }

        internal void DisposeQueue()
        {
            foreach (ActionQueue timer in ActionQueue)
            {
                timer.SafeDispose();
            }

            ActionQueue.Clear();
        }

        // This completely resets all properties and clears all timers
        internal void Reset()
        {
            DisposeQueue();

            foreach (GameProperty property in Properties)
                property.Reset();

            foreach (PlayerData player in Players)
                player.Reset();
        }

        internal ActionQueue GetInQueue(string id)
        {
            return ActionQueue.FirstOrDefault(x => x.Id == id);
        }

        private ActionQueue GetNewestInQueue()
            => GetInQueue(_currentQueuedAction);

        // Invoke an existing action
        internal void InvokeAction(string actionId, InputContext ctx, bool overrideTimer = false)
        {
            if (!overrideTimer)
                if (GetNewestInQueue()?.IsElapsed ?? false)
                    return;

            if (Actions.All(x => x.Id != actionId))
                throw new Exception($"Could not find the specified action '{actionId}'");

            GameAction action = Actions.First(x => x.Id == actionId);

            // TODO: Use InputContext instead of GameContext for input invocations
            action.OnExecute(new GameContext(null, this, _server));

            // this causes a pause, so limit it to the actions that need to update
            if (action.UpdateOnExecute)
                _server.UpdateAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        // Invoke a direct action method instead
        internal void InvokeAction(Action<GameContext> action, bool overrideTimer = false)
        {

        }

        // This is the root action invoked
        // If unspecified

        internal bool LastPendingState { get; set; }
        internal bool PendingUpdate { get; set; }

        internal int CurrentDepth { get; set; } = 0;
        internal int RootDepth { get; set; } = 0;

        internal void InvokeAction(string actionId, bool overrideTimer = false, bool overridePending = false)
        {
            Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] Invoking action {actionId}");

            if (!overrideTimer)
                if (GetNewestInQueue()?.IsElapsed ?? false)
                    return;

            if (Actions.All(x => x.Id != actionId))
                throw new Exception($"Could not find the specified action '{actionId}'");

            GameAction action = Actions.First(x => x.Id == actionId);
            /*
            int baseDepth = RootDepth + 1;

            if (PendingUpdate)
            {
                CurrentDepth++;
            }
            else
            {
                RootDepth = baseDepth;
                CurrentDepth = RootDepth;
                PendingUpdate = action.UpdateOnExecute;
            }*/

            //Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] {PendingUpdate} (root depth {RootDepth}, current depth {CurrentDepth})");
            try
            {
                action.OnExecute(new GameContext(null, this, _server));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _server.DestroyCurrentSession();
                return;
            }
            /*
            if (PendingUpdate)
            {
                if (CurrentDepth == RootDepth)
                {
                    Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] Depth values match, now updating");
                    _server.UpdateAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    PendingUpdate = false;
                    CurrentDepth = 0;
                    RootDepth = 0;
                    return;
                }
                
                Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] An update is already pending");
                CurrentDepth--;
                return;
            }*/

            if (action.UpdateOnExecute)
            {
                _server.UpdateAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        internal bool MeetsCriterion(string ruleId)
        {
            Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] Checking criterion {ruleId}");
            if (Criteria.All(x => x.Id != ruleId))
                throw new Exception($"Could not find the specified rule '{ruleId}'");

            return Criteria.First(x => x.Id == ruleId).Criterion.Invoke(this);
        }

        public GameProperty GetProperty(string id)
        {
            Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] Getting property {id}");

            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not find the specified property '{id}'");

            return Properties.First(x => x.Id == id);
        }

        public GameOption GetOption(string id)
        {
            if (Options.All(x => x.Id != id))
                throw new Exception($"Could not find the specified option '{id}'");

            return Options.First(x => x.Id == id);
        }

        public object GetOptionValue(string id)
            => GetOption(id)?.Value;

        public T GetOptionValue<T>(string id)
        {
            GameOption option = GetOption(id);

            if (option.ValueType == null || !option.ValueType.IsEquivalentTo(typeof(T)))
                throw new Exception("The specified type within the property does not match the implicit type reference");

            return (T)option.Value;
        }

        public void ResetProperty(string id)
            => GetProperty(id)?.Reset();

        public object ValueOf(string id)
            => GetProperty(id)?.Value;

        public T ValueOf<T>(string id)
        {
            var property = GetProperty(id);

            if (property.ValueType != null)
            {
                if (property.ValueType.IsEquivalentTo(typeof(T)))
                {
                    return (T)property.Value;
                }
            }

            throw new Exception("The specified type within the property does not match the implicit type reference");
        }

        public Type TypeOf(string id)
            => GetProperty(id)?.ValueType;

        public void SetValue(string id, object value)
        {
            Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] Setting property {id} to {value.ToString()}");

            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not find the specified property '{id}'");

            Properties.First(x => x.Id == id).Set(value);
        }

        public void SetForEachPlayer(string id, object value)
        {
            foreach (PlayerData player in Players)
                player.SetValue(id, value);
        }

        public void SetValue(string id, string fromId)
        {
            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not the specified property '{id}'");

            if (Properties.All(x => x.Id != fromId))
                throw new Exception($"Could not the specified property '{fromId}'");

            Properties.First(x => x.Id == id).Set(Properties.First(x => x.Id == fromId).Value);
        }

        public void AddToValue(string id, int value)
        {
            Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] Adding {value} to property {id}");

            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not find the specified property '{id}'");

            GameProperty property = Properties.First(x => x.Id == id);

            if (property.ValueType != typeof(int))
                throw new Exception($"Cannot add to attribute '{id}' as it is not a type of Int32");

            property.Value = (int) property.Value + value;
        }

        public PlayerData DataOf(ulong userId)
        {
            Console.WriteLine($"[{Format.Time(DateTime.UtcNow)}] Getting data of {userId}");

            if (Players.All(x => x.Source.User.Id != userId))
                throw new Exception("Cannot find session data for the specified user");

            return Players.First(x => x.Source.User.Id == userId);
        }
    }
}
