﻿using System.Collections.Generic;
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

        public static readonly CardLayout Default = new CardLayout
        {
            Width = 192,
            Height = 32,
            Padding = 2,
            Margin = 2,
            CursorOriginX = 0,
            CursorOriginY = 0,
            CanTrim = true,
            Components = new List<ComponentInfo>
            {
                // Avatar
                new ComponentInfo
                {
                    Type = ComponentType.Image,
                    Group = CardGroup.Avatar,
                    Priority = 0,
                    //MaxWidth = 32,
                    //MaxHeight = 32,
                    //SizeHandling = SizeHandling.Set,
                    Padding = new Padding(right: 2),
                    PrimaryTarget = true,
                    CursorOffset = CursorOffset.X
                },

                // Username
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardGroup.Username,
                    Priority = 1,
                    //MaxWidth = 158, // 192 - 34
                    //SizeHandling = SizeHandling.Throw,
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
                new ComponentInfo // Set the fill info to [FillMode.Bar], specify the Primary and Secondary Gamma, and set the Palette
                {
                    Type = ComponentType.Solid,
                    Group = CardGroup.Level | CardGroup.Exp, // Requires both LEVEL and EXP to be active to display
                    Priority = 5,
                    BaseHeight = 2,
                    Padding = new Padding(right: 5),
                    CursorOffset = CursorOffset.X,
                    OffsetHandling = OffsetHandling.Additive,
                    OffsetUsage = OffsetUsage.Include,
                    PreviousInherit = SizeInherit.Width,
                    PrimaryTarget = true,
                    PreviousOffsetInherit = SizeInherit.Y
                },

                new ComponentInfo // Get the money icon that is used, and set its value to the component info
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

                new ComponentInfo // Get the current amount of money owned, and set its value to the component info.
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

        public bool CanTrim { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int CursorOriginX { get; set; }

        public int CursorOriginY { get; set; }

        public Padding Padding { get; set; }

        public Padding Margin { get; set; }

        // this will be used to created component presets
        public List<ComponentInfo> Components { get; set; }

        // Likewise, this should also be able to understand reading component style IDs and assign them to the layout accordingly
    }
}