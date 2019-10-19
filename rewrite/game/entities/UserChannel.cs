using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
// Import System.Threading.Tasks;
namespace Orikivo
{
    // To verify if the bot can use a user's channel, create a ping message, and if it receives it, update the message to its new.
    // A new test on the user receiver system, used to replace the guild-side receiver.
    public class UserChannel
    {
        // create a user channel
        public UserChannel(SocketUser user)
        {
            Channel = user.GetOrCreateDMChannelAsync().Result; // create user channel.
        }
        public ulong UserId { get; }
        public string Content => Message?.Content;

        // the message id sent; can be null.
        public ulong? MessageId => Message?.Id;

        // used to synchronize channels
        public string SyncKey { get; private set; }

        // the current window of a receiver to point at
        public GameState State { get; private set; }

        // points to the dm of a user.
        private IDMChannel Channel { get; set; }

        // the last sent message update
        private RestUserMessage Message { get; set; }
        
        public async Task<bool> UpdateAsync(SocketUser user, GameDisplay monitor)
        {
            if (user.Id != UserId)
                return false; // the users have to be the same.
            // Handle empty channels, empty messages, if it can update messages...
            if (SyncKey == monitor.GetWindow(State).SyncKey)
                return true;
            await Message.ModifyAsync(x => x.Content = monitor.GetWindow(State).Content);
            throw new NotImplementedException();
        }
        
        public async Task<bool> CloseAsync(string reason = null, TimeSpan? delay = null)
        {
            throw new NotImplementedException();
        }
    }
}
