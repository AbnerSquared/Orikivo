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
        internal readonly GameBuilder Game;

        // create a game session with the information provided
        public GameSession(GameServer server, GameBuilder info)
        {
            StartedAt = DateTime.UtcNow;
            _server = server;
            Game = info;
            Game.SetGameConfig(server);
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

            _server.DisplayChannels.AddRange(info.OnBuildDisplays(Players));

            ActivityDisplay = "playing a game";
        }

        public DateTime StartedAt { get; set; }

        // this is used to display where the game is currently at
        public string ActivityDisplay { get; set; }

        // this is used to handle the current state of a session
        public SessionState State { get; set; } = SessionState.Continue;

        // these are all of the currently active players
        public List<PlayerData> Players { get; internal set; }

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

        internal void QueueAction(TimeSpan delay, string actionId)
        {
            var timer = new ActionQueue(delay, actionId, this);
            ActionQueue.Add(timer);
            _currentQueuedAction = timer.Id;
        }

        internal void QueueAction(string id, TimeSpan delay, string actionId)
        {
            var timer = new ActionQueue(id, delay, actionId, this);
            ActionQueue.Add(timer);
            _currentQueuedAction = timer.Id;
        }

        internal void CancelQueuedAction()
        {
            if (ActionQueue.Count == 0)
                return;

            if (GetCurrentQueuedAction() == null)
                return;

            // marks this timer as cancelled.
            GetCurrentQueuedAction().Cancel();

            ActionQueue.Remove(GetCurrentQueuedAction());
        }

        internal void CancelAllTimers()
        {
            foreach (ActionQueue timer in ActionQueue)
            {
                timer.Cancel();
            }
        }

        internal void DisposeAllTimers()
        {
            foreach (ActionQueue timer in ActionQueue)
            {
                timer.SafeDispose();
            }

            ActionQueue.Clear();
        }

        internal ActionQueue GetQueuedAction(string id)
        {
            return ActionQueue.FirstOrDefault(x => x.Id == id);
        }

        private ActionQueue GetCurrentQueuedAction()
            => GetQueuedAction(_currentQueuedAction);

        internal void InvokeAction(string actionId, InputContext ctx, bool overrideTimer = false)
        {
            if (!overrideTimer)
                if (GetCurrentQueuedAction()?.IsElapsed ?? false)
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

        // This is the root action invoked
        // If unspecified

        internal bool LastPendingState { get; set; }
        internal bool PendingUpdate { get; set; }

        internal int CurrentDepth { get; set; } = 0;
        internal int RootDepth { get; set; } = 0;

        internal void InvokeAction(string actionId, bool overrideTimer = false, bool overridePending = false)
        {
            if (!overrideTimer)
                if (GetCurrentQueuedAction()?.IsElapsed ?? false)
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
            if (Criteria.All(x => x.Id != ruleId))
                throw new Exception($"Could not find the specified rule '{ruleId}'");

            return Criteria.First(x => x.Id == ruleId).Criterion.Invoke(this);
        }

        public GameProperty GetProperty(string id)
        {
            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not find the specified property '{id}'");

            return Properties.First(x => x.Id == id);
        }

        public void ResetProperty(string id)
            => GetProperty(id)?.Reset();

        public object GetPropertyValue(string id)
            => GetProperty(id)?.Value;

        public T GetPropertyValue<T>(string id)
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

        public void SetPropertyValue(string id, object value)
        {
            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not find the specified property '{id}'");

            Properties.First(x => x.Id == id).Set(value);
        }

        public void AddToProperty(string id, int value)
        {
            if (Properties.All(x => x.Id != id))
                throw new Exception($"Could not find the specified property '{id}'");

            GameProperty property = Properties.First(x => x.Id == id);

            if (property.ValueType != typeof(int))
                throw new Exception($"Cannot add to attribute '{id}' as it is not a type of Int32");

            property.Value = (int) property.Value + value;
        }

        public PlayerData GetPlayerData(ulong userId)
        {
            if (Players.All(x => x.Player.User.Id != userId))
                throw new Exception("Cannot find session data for the specified user");

            return Players.First(x => x.Player.User.Id == userId);
        }
    }
}
