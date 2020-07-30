using Discord;
using System.Threading.Tasks;

namespace Arcadia.Multiplayer

{
    /// <summary>
    /// Represents a generic direct channel to a <see cref="Player"/>.
    /// </summary>
    public class PlayerChannel
    {
        // this auto-sets up a player channel with the specified player and display channel.
        public static async Task<PlayerChannel> CreateAsync(Player player, DisplayChannel channel)
        {
            IDMChannel dm = await player.User.GetOrCreateDMChannelAsync();

            PlayerChannel result = new PlayerChannel
            {
                User = player.User,
                Frequency = channel.Frequency,
                InternalChannel = dm,
                InternalMessage = await dm.SendMessageAsync(channel.ToString())
            };

            return result;
        }

        // who is the user i am bound to?
        public IUser User { get; private set; }

        public IDMChannel InternalChannel { get; private set; }

        // what message should i update?
        public IUserMessage InternalMessage { get; internal set; }

        // what is the frequency of the display i am currently looking for?
        public int Frequency { get; internal set; }
    }
}
