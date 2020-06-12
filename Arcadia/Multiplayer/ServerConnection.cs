using Discord;

namespace Arcadia

{
    public class ServerConnection
    {
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

        // is this channel being used in a session right now?
        // if so, ignore all commands being used here.
        public bool Playing { get; set; } = false;
    }
}
