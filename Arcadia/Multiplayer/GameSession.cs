using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    public class GameSession
    {
        internal GameSession()
        {

        }

        // create a game session with the information provided
        public GameSession(GameServer server, GameBuilder info)
        {

        }

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

        // a list of custom players
        // a list of custom attributes
        // a method handler for everything that happens in-game
    }
}
