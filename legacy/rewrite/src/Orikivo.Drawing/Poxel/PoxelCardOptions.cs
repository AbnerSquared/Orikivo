using Discord.WebSocket;

namespace Orikivo.rewrite.src.Orikivo.Drawing.Poxel
{
    public class PoxelCardOptions
    {
        public PoxelCardOptions(PoxelUnitScale scale, SocketUser user)
        {
            //if (Container.TryGetOriUser(user, out OriUser oriUser))
               // User = new PoxelCardUserData(user, oriUser);

        }


        public PoxelCardFormatOptions Format { get; } // determines what components are being rendered or not.
        public PoxelColorPacket Packet { get; } // color packet that user set.
        public PoxelCardUserData User { get; } // the user that is being used on the card


    }
}
