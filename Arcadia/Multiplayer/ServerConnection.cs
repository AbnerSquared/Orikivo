using Discord;
using System;

namespace Arcadia
{
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
