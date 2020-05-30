using Discord;

namespace Arcadia

{
    public class ServerConnection
    {
        public ulong ChannelId;
        public ulong MessageId; 

        // only 1 channel can be bound to a server at a time
        // check to see if the specified channel is connected to anything else.
        // likewise, you can simply create a cache of channels with their ID and server ID
        // where should i listen to input?
        public IMessageChannel InternalChannel;

        // what message should i update?
        public IUserMessage InternalMessage;

        // what is the frequency of the display am I currently pointing to?
        public int Frequency;
    }
}
