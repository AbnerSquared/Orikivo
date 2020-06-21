using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;


namespace Arcadia
{
    internal class TimerData
    {
        internal GameSession Session { get; set; }
    }
    public class GameTimer : IDisposable
    {
        public GameTimer()
        {

        }

        private Timer InternalTimer { get; set; }

        public void Run(TimeSpan duration, GameSession session)
        {
            TimerCallback callback = new TimerCallback(x => (x as GameSession).TimeInvoke.SetResult(true));
            
            // callback: This is the method that will be executed once the timer waits for the specified duration
            // state: This is the object that is passed to the callback whenever the timer waits for the specified duration
            // dueTime: This is the delay to be set on the timer before the method is invoked
            // period: This is the interval at which the timer constantly invokes the next method.
            // you can probably compare timer IDs, and if they don't match, cancel the invocation.
            InternalTimer = new Timer(callback, session, duration, TimeSpan.FromMilliseconds(-1));
        }

        private delegate void TimerInfo(GameSession session, bool isDisposed);

        private bool Disposed { get; set; }

        public void Dispose()
        {
            Disposed = true;

        }
    }

    public enum TimerState
    {
        // the timer has started.
        Started = 1,

        // the timer has met their goal
        Elasped = 2,

        // the timer was paused
        Paused = 4,
        // the timer never started
        Inactive = 8
    }

    public class GameSession
    {
        internal GameSession()
        {

        }

        // create a game session with the information provided
        public GameSession(GameServer server, GameBuilder info)
        {

        }

        // this is used to display where the game is currently at
        public string ActivityBuffer { get; set; }

        // what channel does anyone spectating focus on
        public int WatchingFrequency { get; set; }

        // what channel does everyone playing focus on
        public int PlayingFrequency { get; set; }

        // this is used to handle the current state of a session
        public SessionState State { get; set; } = SessionState.Continue;
        
        // Playing Trivia (Question 10/15)

        // invoke to execute a timer's data
        internal TaskCompletionSource<bool> TimeInvoke { get; set; }
        
        // invoke to execute the end of the session
        internal TaskCompletionSource<bool> EndInvoke { get; set; }


        // this is a timer that can be constantly set up
        // the elapsed event is connected to a game action
        public AsyncTimer Timer { get; internal set; }

        // This is the action ID executed once the timer is elapsed
        public string OnTimer { get; internal set; }

        // These are all of the rulesets read each time an input is executed
        // if one of them are true, the action specified is executed
        // The action specified is <bool, GameSession>; the boolean value is true if the rule was true, and gamesession allows you to edit its properties
        public List<RuleAction> Rulesets { get; internal set; }

        // a list of all display channels
        public List<DisplayChannel> Displays { get; internal set; }

        // these are all of the currently active players
        public List<PlayerSessionData> Players { get; internal set; }

        // these are all of the attributes that are set
        public List<GameProperty> Attributes { get; internal set; }

        // These are all of the possible actions
        public List<GameAction> Actions { get; set; }

        // these are all of the possible rules
        public List<GameRule> Rules { get; set; }

        // this can be called to set up a new timer
        // if an existing timer is already set, that one is deleted
        // and replaced with the new one
        internal void SetTimer(TimeSpan duration, string actionId)
        {
            // a timer starts once the await server.UpdateAsync() is called.
        }

        // this starts the timer.
        internal void StartTimer()
        {

        }

        // this can be called to completely cancel and remove a timer
        internal void CancelTimer()
        {

        }

        // this can be called to reset a timer
        internal void ResetTimer()
        {

        }

        internal void ExecuteAction(string actionId)
        {
            if (!Actions.Any(x => x.Id == actionId))
                throw new Exception($"Cannot find the action '{actionId}' specified");

            // Actions.First(x => x.Id == actionId).OnExecute(this);
        }

        internal void ExecuteRule(string ruleId)
        {
            if (!Rules.Any(x => x.Id == ruleId))
                throw new Exception($"Cannot find the rule '{ruleId}' specified");

            Rules.First(x => x.Id == ruleId).Criterion.Invoke(this);
        }

        public GameProperty GetAttribute(string id)
        {
            if (!Attributes.Any(x => x.Id == id))
                throw new Exception($"Cannot find the attribute '{id}' specified");

            return Attributes.First(x => x.Id == id);
        }

        public void SetAttribute(string id, object value)
        {
            if (!Attributes.Any(x => x.Id == id))
                throw new Exception($"Cannot find the attribute '{id}' specified");

            Attributes.First(x => x.Id == id).Set(value);
        }

        public void AddToAttribute(string id, int value)
        {
            if (!Attributes.Any(x => x.Id == id))
                throw new Exception($"Cannot find the attribute '{id}' specified");

            var attribute = Attributes.First(x => x.Id == id);

            if (attribute.ValueType != typeof(int))
                throw new Exception($"Cannot add to attribute '{id}' as it is not a type of Int32");

            attribute.Value = ((int)attribute.Value) + value;
        }


        public DisplayChannel GetDisplay(int frequency)
        {
            if (!Displays.Any(x => x.Frequency == frequency))
                throw new Exception($"Cannot find a display at frequency {frequency}");

            return Displays.First(x => x.Frequency == frequency);
        }

        public PlayerSessionData GetPlayerData(Player player)
        {
            if (!Players.Any(x => x.Player == player))
                throw new Exception("Cannot find a matching session data for the specified player");

            return Players.First(x => x.Player == player);
        }

        public async Task RunAsync()
        {

        }

        // a list of custom players
        // a list of custom attributes
        // a method handler for everything that happens in-game
    }

    public enum SessionState
    {
        Continue = 1,
        Finish = 2,
        Destroy = 4
    }
}
