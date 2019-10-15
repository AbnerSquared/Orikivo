using System;
// Import System.Threading.Tasks;
namespace Orikivo
{
    /// <summary>
    /// A generic user that defines their back-end game state.
    /// </summary>
    public class User
    {
        public User(ulong id, string name, ulong guildId, UserTag tags = UserTag.Empty)
        {
            Id = id;
            Name = name;
            ReceiverId = guildId;
            if (tags != UserTag.Empty)
                Tags |= tags;
            else
                Tags = tags;
            JoinedAt = DateTime.UtcNow;
        }
        public ulong Id { get; }
        public string Name { get; }
        public GameState State { get; internal set; }
        // Incomplete // public UserReceiver Receiver { get; }
        public UserTag Tags { get; internal set; }
        public bool IsHost => Tags.HasFlag(UserTag.Host);
        public ulong ReceiverId { get; internal set; }
        public DateTime JoinedAt { get; }
    }
    
    // To verify if the bot can use a user's channel, create a ping message, and if it receives it, update the message to its new.
    // A new test on the user receiver system, used to replace the guild-side receiver.
    public class UserReceiver
    {
        public ulong UserId { get; }
        // The Id that is used to connect to the channel aforementioned. Should just be Username#Discriminator
        public string Id { get; }
        public bool CanUpdateMessage { get; }
        public string Content => Message?.Content;
        public ulong? ChannelId => Channel?.Id;
        public ulong? MessageId => Message?.Id;
        public string SyncKey { get; private set; }
        private IDMChannel Channel { get; set; }
        private RestUserMessage Message { get; set; }
        
        public async Task<bool> UpdateAsync(BaseSocketClient client, GameMonitor monitor)
        {
            // Handle empty channels, empty messages, if it can update messages...
            await Channel.UpdateAsync();
            if (Channel is null)
               // await client.GetDMChannelAsync(Id);
            if (SyncKey == monitor.GetWindow(State).SyncKey)
                return true;
            await Message.ModifyAsync(x => x.Text = monitor.GetWindow(State).Content);
        }
        
        public async Task<bool> CloseAsync(string reason = null, TimeSpan delay = null)
        {
            throw new NotImplementedException();
        }
    }
}
