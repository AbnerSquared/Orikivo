using Discord;
using System;

namespace Arcadia

{
    public enum GameState
    {
        Waiting = 1, // currently in the lobby
        /*
            inputs:
            - start: [requires: host] this starts the game if there are enough players.
                if a game is already started or the criteria isn't met, this doesn't happen

            - join: this allows a user to join the current lobby.
                if they already joined, just update the lobby to reflect that

            - leave: this allows a user to leave the current lobby.
                if they aren't in the lobby, the message is ignored

            - config: [requires: host] this allows the host to edit the current configurations for the server
                this changes the channel in which it was executed in to Editing
                if there is a connection that is already in editing, it will not allow this command to execute again.

            - players: this allows a player to view all of the current players in a server
                this simply sends a message to the console that lists all players
         
            - watch: this starts a vote counter for all current players in the lobby to watch the current game.
                if a game hasn't started yet, this input will be hidden and not be allowed to be used
                if enough players vote, this sets all channels in the Waiting state to the Watching state.
         */
        Editing = 2, // currently editing configurations for the server
        /*
            display:
            - header: This simply shows the header of the configuration menu. [0:title]
                Editing Abner's Server
            - subconsole: This simply shows a mini version of the console in the main lobby
                ```
                * [0]
                * [1]
                * [2]
                * [3]
                ```
            - main_config: this shows all of the available configurations for the lobby.
                
            **Config**
            
                **Title**: Represents the name of the server.
                > `Abner's Server`
                **Privacy**: Represents the visibility of this server.
                > `Public`
                **Game**: Represents the game that will be played.
                > `Trivia`

            - game_config: this shows all of the available configurations for the game specified.
                if a game isn't valid, this component will be marked as invalid.

            **Trivia**
            
                **Topics**: Sets the topics that will be used for trivia.
                > `All`
                **Difficulty**: Sets the max difficulty for the questions that appear.
                > `Hard`
                **Question Duration**: Sets the amount of time a question can idle for.
                > `30sec.`
                **Question Count**: Sets the amount of questions to answer.
                > `5`

            inputs:
            - back: [requires: host] this returns the game server to the lobby.

            - title <value>: [requires: host] this changes the game server title to the one provided.
                if there isn't a value provided, it won't update.

            - privacy <mode>: [requires: host] this changes the game server privacy to the one provided.
                if there isn't a value provided, it won't update.

            - game <id>: [requires: host] this changes the game for the server to the one specified.
                if there isn't a value provided, it will show the list of all available games

            - reset: [requires: host] this sets all of the configurations to their default again.

            if a game id specified points to a valid game, it will then list all of the configurations for this mode:

            If a specified input doesn't lead to any default input, it will then attempt to find a custom config match.
            - custom_config <value>: [requires: host] this changes a specific config for the specified game to the one specified.
                if a value isn't provided, it will specify all of the correct values
                if an incorrect value is given, it will let you know that it is.
         
         */

        Watching = 4, // currently watching the game (uses the same display channel, but can't use input
        /*
            display:
                - header: this is the header used for spectating, just to let others know.
                    > **You are currently spectating the game.**

                - buffer: this is the buffer derived from the main display channel the game uses.

                - footer: this is the footer for the display that's used
                    `back` (0/4 players voted)
            inputs:
            - back: starts a vote for everyone to return to the lobby. if enough players vote, it will return to the lobby.
         
         */
        Playing = 8 // currently a part of the session
        /*
            display: this is customized for each game that will be played
            inputs: these are customized for each game that will be played
         
         */
    }
    public class ServerConnection
    {
        // every 4 messages, the InternalMessage will be updated.
        private static int _afterMessageLimit => 4;
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }

        // only 1 channel can be bound to a server at a time
        // check to see if the specified channel is connected to anything else.
        // likewise, you can simply create a cache of channels with their ID and server ID
        // where should i listen to input?
        public IMessageChannel InternalChannel { get; set; }

        // what message should i update?
        public IUserMessage InternalMessage { get; set; }

        // what is the frequency of the display am I currently pointing to?
        public int Frequency { get; set; }

        // when was the last time this connection was refreshed
        // if the refreshrate is specified, if the time since the last refresh is shorter
        // don't refresh the console. likewise, you can create an async refresh that refreshes automatically
        // once the time to refresh has been met.
        internal DateTime LastRefreshed { get; set; }

        // this keeps track of how many messages were sent after the server message was sent.
        // if enough messages are sent after the lobby message and
        // can delete messages is false
        // a new message is sent in replacement
        internal int AfterMessageCount { get; set; }

        // determines if the bot can delete messages
        public bool CanDeleteMessages { get; set; } = false;

        // this determines what is currently being executed in the server connection
        public GameState State { get; set; }
    }
}
