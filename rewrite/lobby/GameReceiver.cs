using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Orikivo
{
    // a subscriber to the game that reads off of the game's display.
    public class GameReceiver
    {
        // reference the GameEventHandler to determine when a user leaves and whatknot.
        private GameEventHandler _eventHandler;

        // the display id to listen for. this should default to lobby
        // the receiver gamestate it's currently in.
        // the guild it's for
        // the event handler
        // the list of current users
        // the config for the receiver.
        public GameReceiver(SocketGuild guild, GameEventHandler eventHandler, List<User> users, ReceiverConfig config)
        {
        }

        // handle UserJoined, UserLeft, DisplayUpdated
        // DisplayUpdated can handle updating receivers, but only if the 
        // receiver is listening to the display that was updated.

            // this determines what display to listen for
        public int DisplayId { get; private set; }
        public ulong Id { get; } // this is the guild id
        public string Name { get; } // the name used for the receiver.
        public bool CanDeleteMessages { get; } // if the receiver allows for deleting messages.
        public bool CanUpdateMessage { get; } // if the receiver updates the existing message in a lobby.

        // this determines what the receiver is looking for.
        public GameState State { get; private set; }

        // find a new way to ensure synchronization
        public string SyncKey { get; private set; }

        // the channel info of the receiver.
        private RestTextChannel Channel { get; set; }
        public ulong? ChannelId => Channel?.Id;
        // the message info of the receiver. if !CanUpdateMessage, this is ignored.
        private RestUserMessage Message { get; set; }

        // this is all of the users bound to this receiver.
        // this is immutable to ensure it's readonly from the lobby itself.
        public ImmutableArray<ulong> UserIds { get; }
        public string Mention => Channel?.Mention;

        // updates the receiver with the most current info
        public async Task UpdateAsync(BaseSocketClient client, Display display)
        { }

        // deletes the receiver
        public async Task DeleteAsync(string reason = null, TimeSpan? timeout = null)
        {

        }

    }
}
