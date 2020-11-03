using System.Collections.Generic;
using System.Drawing;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    /// <summary>
    /// Represents the layout details of a card.
    /// </summary>
    public class CardLayout
    {
        public const int MaxImageWidth = 400;
        public const int MaxImageHeight = 300;

        public static readonly CardLayout Micro = new CardLayout
        {
            Width = 200,
            Height = 16,
            TrimMode = TrimMode.Force,
            Margin = 2,
            Padding = new Padding(left: 2),
            BorderAllow = BorderAllow.Left,
            BorderThickness = 2,
            BorderEdge = BorderEdge.Outside,
            BorderFill = new BaseFillInfo
            {
                Mode = FillMode.Solid,
                Primary = Gamma.Max
            },
            CursorOriginX = 0,
            CursorOriginY = 0,
            AvatarScale = ImageScale.Small,
            Components = new List<ComponentInfo>
            {
                // AVATAR
                new ComponentInfo
                {
                    Priority = 0,
                    PrimaryTarget = true,
                    Type = ComponentType.Image,
                    Group = CardGroup.Avatar,
                    Padding = new Padding(right: 2),
                    CursorOffset = CursorOffset.X
                },

                // NAME
                new ComponentInfo
                {
                    Priority = 1,
                    PrimaryTarget = true,
                    Type = ComponentType.Text,
                    Group = CardGroup.Name | CardGroup.Exp,
                    Padding = new Padding(bottom: 1),
                    CursorOffset = CursorOffset.Y,
                    BaseFill = new BaseFillInfo
                    {
                        Mode = FillMode.Bar,
                        Primary = Gamma.Max,
                        Secondary = Gamma.Bright,
                        Direction = Direction.Up
                    },
                    BaseOutline = new BaseFillInfo
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
                    Priority = 2,
                    PrimaryTarget = true,
                    Type = ComponentType.Text,
                    Group = CardGroup.Activity,
                    CursorOffset = CursorOffset.Y,
                    BaseFill = new BaseFillInfo
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
            BorderAllow = BorderAllow.All,
            BorderThickness = 2,
            BorderEdge = BorderEdge.Outside,
            BorderFill = new BaseFillInfo
            {
                Mode = FillMode.Solid,
                Primary = Gamma.Max
            },
            AvatarScale = ImageScale.Medium,
            Components = new List<ComponentInfo>
            {
                // Avatar
                new ComponentInfo
                {
                    Type = ComponentType.Image,
                    Group = CardGroup.Avatar,
                    Priority = 0,
                    Padding = new Padding(right: 2),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.X
                },

                // Username
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardGroup.Name,
                    Priority = 1,
                    Padding = new Padding(bottom: 2),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.Y
                },

                // Activity
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardGroup.Activity,
                    Priority = 2,
                    Padding = new Padding(bottom: 2),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.Y
                },

                // Level Icon
                new ComponentInfo
                {
                    Type = ComponentType.Icon,
                    Group = CardGroup.Level,
                    Priority = 3,
                    Padding = new Padding(right: 1),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.X
                },

                // Level Counter
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardGroup.Level,
                    Priority = 4,
                    Padding = new Padding(right: 5, bottom: 1),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.None
                },

                // Exp bar
                new ComponentInfo
                {
                    Type = ComponentType.Solid,
                    Group = CardGroup.Level | CardGroup.Exp,
                    Priority = 5,
                    BaseHeight = 2,
                    Padding = new Padding(right: 5),
                    CursorOffset = CursorOffset.X,
                    OffsetHandling = OffsetHandling.Additive,
                    OffsetUsage = OffsetUsage.Include,
                    PreviousInherit = SizeInherit.Width,
                    PrimaryTarget = true,
                    PreviousOffsetInherit = SizeInherit.Y,
                    BaseFill = new BaseFillInfo
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
                    Priority = 6,
                    Padding = new Padding(right: 2),
                    CursorOffset = CursorOffset.X,
                    OffsetHandling = OffsetHandling.Additive,
                    OffsetUsage = OffsetUsage.Temporary,
                    PrimaryTarget = true,
                    OffsetY = -1
                },

                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardGroup.Money,
                    Priority = 7,
                    Padding = new Padding(right: 1),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.X
                }
            }
        };

        public ImageScale AvatarScale { get; set; } = ImageScale.Medium;

        public TrimMode TrimMode { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int CursorOriginX { get; set; }

        public int CursorOriginY { get; set; }

        public Padding Padding { get; set; }

        public Padding Margin { get; set; }

        public BorderAllow BorderAllow { get; set; }

        public BorderEdge BorderEdge { get; set; } = BorderEdge.Outside;

        public int BorderThickness { get; set; }

        public BaseFillInfo BorderFill { get; set; }

        public List<ComponentInfo> Components { get; set; }
    }
}