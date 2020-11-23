using System.Collections.Generic;
using Discord;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a direct connection to a <see cref="Player"/>.
    /// </summary>
    public class PlayerConnection
    {
        // this auto-sets up a player channel with the specified player and display channel.
        public static async Task<PlayerConnection> CreateAsync(Player player, GameBroadcast channel)
        {
            IDMChannel dm = await player.User.GetOrCreateDMChannelAsync();

            var result = new PlayerConnection
            {
                User = player.User,
                Frequency = channel.Frequency,
                InternalChannel = dm,
                InternalMessage = await dm.SendMessageAsync(channel.ToString())
            };

            return result;
        }

        public ConnectionType Type => ConnectionType.Direct;

        // who is the user i am bound to?
        public IUser User { get; private set; }

        public IDMChannel InternalChannel { get; private set; }

        // what message should i update?
        public IUserMessage InternalMessage { get; internal set; }

        // what is the frequency of the display i am currently looking for?
        // If unspecified, the Content/Inputs will be referred to instead
        public int Frequency { get; internal set; }

        public DisplayContent Content { get; internal set; }

        public List<IInput> Inputs { get; internal set; } = new List<IInput>();
    }
}
