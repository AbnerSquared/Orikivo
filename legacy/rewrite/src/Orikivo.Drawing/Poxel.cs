using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Orikivo
{
    public class ColorPacketManager
    {
        public static OriColorPacket GetPacket(uint packetId)
        {
            return null;
        }
    }

    public class Poxel : IDisposable
    {
        public Poxel(PoxelRenderingOptions options)
        {
            Font = FontManager.FontMap.GetFont(options.FontId);
            Colors = ColorPacketManager.GetPacket(options.PacketId);
            AlphaColor = options.AlphaColor;
        }

        public void Write(string value) { } // Bitmap => void
        public void Write(Bitmap image, Point point, string value) { } // Bitmap => void
        public void GetAvatar(SocketUser user, PoxelCardAvatarFormat avatarFormat) { } // Bitmap => void
        public PoxelCard GetCard(Account account) { return new PoxelCard(); }
        public byte Scale { get; }
        public FontFace Font { get; }
        public OriColorPacket Colors { get; }
        public OriColor AlphaColor { get; } // can be null
        public void Dispose() { } // clear out all items.
    }

    public class PoxelUnitSize
    {
        public byte LengthWidth { get; set; }
    }

    public class OriColorPacket
    {
        [JsonProperty("id")]
        uint Id { get; set; }

        [JsonProperty("socket_id")]
        uint? SocketId { get; set; } // if an OriItem has this ID, the color will be mapped to that OriItem.

        [JsonProperty("map")]
        Dictionary<int, OriColor> Map { get; }
    }

    // a compressed idealistic viewpoint of options.
    public class PoxelRenderingOptions
    {
        public byte Scale { get; }
        public uint FontId { get; }
        public uint PacketId { get; }
        public OriColor AlphaColor { get; }
    }

    public class PoxelCardFormatOptions
    {
        public PoxelCardAvatarFormat AvatarFormat { get; private set; }
        public PoxelCardNameFormat NameFormat { get; private set; }
    }

    public class PoxelCard
    {
        public Dictionary<Point, Bitmap> Components { get; private set; }
        public void AddComponent(PoxelCardComponent component)
            => Components.Add(component.Point, component.Sprite);
    }

    // a pre-rendered component, ready to be placed onto a PoxelCard.
    public class PoxelCardComponent
    {
        public Point Point { get; }
        public Bitmap Sprite { get; }
    }

    public class PoxelCardRenderingOptions
    {
        
    }

    // the card of the user being rendered.
    public class PoxelCardUserData
    {
        public PoxelCardUserData(SocketUser user, Account account)
        {
            Name = account.Name;
            Activity = user.Activity;
            Experience = account.Experience;
            Balance = account.Balance;
            Debt = account.Debt;
        }

        public string Name { get; }
        public IActivity Activity { get; }
        public ulong Experience { get; }
        public ulong Balance { get; }
        public ulong Debt { get; }
    }

    public enum PoxelCardNameFormat
    {
        Dynamic = 0, // Whatever is set for the user being rendered.
        Default = 1, // Username#Discrim
        Id = 2 // UserId
    }

    public enum PoxelCardAvatarFormat
    {
        Default = 0, // 32u
        Full = 1, // 256u
        Minimized = 2 // 16u
    }

    public class PoxelCardBorder
    {
        public string SpritePath { get; }
        public bool AutoCrop { get; }
        public PoxelUnitSize Size { get; }
    }

    public class PoxelFontFace
    {

    }
}
