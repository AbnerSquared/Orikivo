using System.Collections.Generic;
using Newtonsoft.Json;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Represents the layout details of a card.
    /// </summary>
    public class CardLayout
    {
        /// <summary>
        /// Represents the maximum width of a card.
        /// </summary>
        public const int MaxImageWidth = 400;

        /// <summary>
        /// Represents the maximum height of a card.
        /// </summary>
        public const int MaxImageHeight = 300;

        // TODO: Handle layout definitions in another class
        public static readonly CardLayout Micro = new CardLayout
        {
            Width = 200,
            Height = 16,
            TrimMode = TrimMode.Force,
            Margin = 2,
            Padding = new Padding(left: 2),
            Border = new BaseBorderInfo
            {
                Allowed = BorderAllow.Left,
                Thickness = 2,
                Edge = BorderEdge.Outside,
                Fill = new BaseFillInfo
                {
                    Mode = FillMode.Solid,
                    Primary = Gamma.Max
                }
            },
            CursorOriginX = 0,
            CursorOriginY = 0,
            AvatarScale = ImageScale.Small,
            Components = new List<ComponentInfo>
            {
                // AVATAR
                new ComponentInfo
                {
                    Position = 0,
                    PrimaryTarget = true,
                    Type = ComponentType.Image,
                    Group = CardGroup.Avatar,
                    Padding = new Padding(right: 2),
                    CursorOffset = CursorOffset.X
                },

                // NAME
                new ComponentInfo
                {
                    Position = 1,
                    PrimaryTarget = true,
                    Type = ComponentType.Text,
                    Group = CardGroup.Name | CardGroup.Exp,
                    Padding = new Padding(bottom: 1),
                    CursorOffset = CursorOffset.Y,
                    Fill = new BaseFillInfo
                    {
                        Mode = FillMode.Bar,
                        Primary = Gamma.Max,
                        Secondary = Gamma.Bright,
                        Direction = Direction.Up
                    },
                    Outline = new BaseFillInfo
                    {
                        Mode = FillMode.Bar,
                        Primary = Gamma.Standard,
                        Secondary = Gamma.Dim,
                        Direction = Direction.Up
                    }
                },

                // ACTIVITY
                new ComponentInfo
                {
                    Position = 2,
                    PrimaryTarget = true,
                    Type = ComponentType.Text,
                    Group = CardGroup.Activity,
                    CursorOffset = CursorOffset.Y,
                    Fill = new BaseFillInfo
                    {
                        Mode = FillMode.Solid,
                        Primary = Gamma.Max
                    }
                }
            }
        };

        public static readonly CardLayout Default = new CardLayout
        {
            Width = 192,
            Height = 32,
            Padding = 2,
            Margin = 2,
            CursorOriginX = 0,
            CursorOriginY = 0,
            TrimMode = TrimMode.Optional,
            Border = new BaseBorderInfo
            {
                Allowed = BorderAllow.All,
                Thickness = 2,
                Edge = BorderEdge.Outside,
                Fill = new BaseFillInfo
                {
                    Mode = FillMode.Solid,
                    Primary = Gamma.Max
                }
            },
            AvatarScale = ImageScale.Medium,
            Components = new List<ComponentInfo>
            {
                // Avatar
                new ComponentInfo
                {
                    Type = ComponentType.Image,
                    Group = CardGroup.Avatar,
                    Position = 0,
                    Padding = new Padding(right: 2),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.X
                },

                // Username
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardGroup.Name,
                    Position = 1,
                    Padding = new Padding(bottom: 2),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.Y
                },

                // Activity
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardGroup.Activity,
                    Position = 2,
                    Padding = new Padding(bottom: 2),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.Y
                },

                // Level Icon
                new ComponentInfo
                {
                    Type = ComponentType.Icon,
                    Group = CardGroup.Level,
                    Position = 3,
                    Padding = new Padding(right: 1),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.X
                },

                // Level Counter
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardGroup.Level,
                    Position = 4,
                    Padding = new Padding(right: 5, bottom: 1),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.None
                },

                // Exp bar
                new ComponentInfo
                {
                    Type = ComponentType.Solid,
                    Group = CardGroup.Level | CardGroup.Exp,
                    Position = 5,
                    BaseHeight = 2,
                    Padding = new Padding(right: 5),
                    CursorOffset = CursorOffset.X,
                    OffsetHandling = OffsetHandling.Additive,
                    OffsetFinalize = OffsetFinalize.Keep,
                    SizeInherit = SizeInherit.Width,
                    PrimaryTarget = true,
                    OffsetInherit = SizeInherit.Y,
                    Fill = new BaseFillInfo
                    {
                        Mode = FillMode.Bar,
                        Primary = Gamma.Max,
                        Secondary = Gamma.Standard,
                        Direction = Direction.Right
                    },
                },

                new ComponentInfo
                {
                    Type = ComponentType.Icon,
                    Group = CardGroup.Money,
                    Position = 6,
                    Padding = new Padding(right: 2),
                    CursorOffset = CursorOffset.X,
                    OffsetHandling = OffsetHandling.Additive,
                    OffsetFinalize = OffsetFinalize.Ignore,
                    PrimaryTarget = true,
                    OffsetY = -1
                },

                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardGroup.Money,
                    Position = 7,
                    Padding = new Padding(right: 1),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.X
                }
            }
        };

        // TODO: Handle scaling in ImageComponent instead
        /// <summary>
        /// Defines the avatar build scale for this layout.
        /// </summary>
        [JsonProperty("avatar_scale")]
        public ImageScale AvatarScale { get; set; } = ImageScale.Medium;

        /// <summary>
        /// Defines the trimming mode for this layout.
        /// </summary>
        [JsonProperty("trim_mode")]
        public TrimMode TrimMode { get; set; }

        /// <summary>
        /// Defines the initial width of this layout.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// Defines the initial height of this layout.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// Defines the horizontal cursor origin of this layout.
        /// </summary>
        [JsonProperty("cursor_origin_x")]
        public int CursorOriginX { get; set; }

        /// <summary>
        /// Defines the vertical cursor origin of this layout.
        /// </summary>
        [JsonProperty("cursor_origin_y")]
        public int CursorOriginY { get; set; }

        /// <summary>
        /// Defines the padding of this layout.
        /// </summary>
        [JsonProperty("padding")]
        public Padding Padding { get; set; }

        /// <summary>
        /// Defines the border of this layout.
        /// </summary>
        [JsonProperty("border")]
        public BaseBorderInfo Border { get; set; }

        /// <summary>
        /// Defines the margin of this layout.
        /// </summary>
        [JsonProperty("margin")]
        public Padding Margin { get; set; }

        /// <summary>
        /// Defines the collection of components to be rendered on this layout.
        /// </summary>
        [JsonProperty("components")]
        public List<ComponentInfo> Components { get; set; }
    }
}
