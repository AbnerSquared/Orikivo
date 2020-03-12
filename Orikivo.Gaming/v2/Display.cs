using Discord;
using Orikivo.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo.Gaming
{
    // represents an emitter for the game display.
    public class Broadcast : IBroadcast
    {
        // the display to broadcast
        public Display Display { get; }

        // all of the receivers subscribed to this broadcast.

        private List<Receiver> _receivers;

        public IReadOnlyList<Receiver> Receivers => _receivers;

        public async Task AddAsync(IMessageChannel channel)
        {
            var receiver = await Receiver.CreateAsync(channel, Display);
            _receivers.Add(receiver);
        }
    }

    // represents a display bound to a single channel.
    public class Unicast : IBroadcast
    {
        private Unicast(Display display, Receiver receiver)
        {
            Display = display;
            Receiver = receiver;
        }

        public static async Task<Unicast> CreateAsync(IMessageChannel channel, Display display)
        {
            Receiver receiver = await Receiver.CreateAsync(channel, display);
            return new Unicast(display, receiver);
        }

        public Display Display { get; }
        public Receiver Receiver { get; }
    }

    public interface IBroadcast
    {
        Display Display { get; }
    }

    // represents a connection to a broadcast
    public class Receiver
    {
        private IMessageChannel _channel;
        private IUserMessage _message;

        internal static async Task<Receiver> CreateAsync(IMessageChannel channel, Display display)
        {
            var message = await channel.SendMessageAsync(display.Draw());
            var receiver = new Receiver() { _channel = channel, _message = message };
            
            return receiver;
        }

    }

    public class Display
    {
        // used to identify the display
        public string Id { get; }

        public IReadOnlyList<DisplayNode> Nodes { get; }

        // formats and draws all of the nodes.
        public string Draw()
        {
            return "";
        }
    }


    public class DisplayNode : ContentNode
    {
        public string Id { get; }

        protected override string Formatting => "";
    }
}
