using System;
using System.Collections.Generic;
using System.Drawing;
using Orikivo.Drawing;
using Color = Discord.Color;

namespace Arcadia.Graphics
{
    [Flags]
    public enum CardComponent
    {
        Username = 1,
        Activity = 2,
        Avatar = 4,
        Level = 8,
        Money = 16,
        Exp = 32,
        Bar = 64
    }

    public enum ComponentType
    {
        Image = 1,
        Icon = 2,
        Text = 3,
        Solid = 4
    }

    // Likewise, this is also handled by Merit slots
    public class IconComponent : ICardComponent
    {
        public ComponentInfo Info { get; }
        public FillInfo Fill { get; }

        public string ReferencePath { get; set; }
        public int ReferenceIndex { get; set; }
        public int CropWidth { get; set; }
        public int CropHeight { get; set; }

        public void Draw(Drawable card, ref Cursor cursor, ref ComponentReference previous)
        {
            // Get the layer offsets
            int offsetX = CardInfo.GetOffsetX(Info, cursor, previous);
            int offsetY = CardInfo.GetOffsetY(Info, cursor, previous);

            // Get the image source
            var sheet = new Sheet(ReferencePath, CropWidth, CropHeight);
            var sprite = sheet.GetSprite(ReferenceIndex);
            var icon = ImageHelper.SetColorMap(sprite, GammaPalette.Default, Fill.Palette);

            // Build the new layer
            var layer = new BitmapLayer(icon)
            {
                Offset = new Coordinate(offsetX, offsetY)
            };

            layer.Properties.Padding = Info.Padding;

            // Try to offset the cursor
            if (Info.CursorOffset.HasFlag(CursorOffset.X))
                cursor.X += layer.Source.Width + layer.Properties.Padding.Width;

            if (Info.CursorOffset.HasFlag(CursorOffset.Y))
                cursor.Y += layer.Source.Height + layer.Properties.Padding.Height;

            // Finally, update the component reference
            previous.Update(layer.Source.Width, layer.Source.Height, layer.Properties.Padding);

            // Add the layer to the card
            card.AddLayer(layer);
        }
    }

    public class FrameComponent
    {
        public string ReferencePath;
        public int ReferenceIndex;
        public int CropWidth;
        public int CropHeight;
        public GammaPalette Palette;
        // This defines what this frame is meant for, so that it can't be incorrectly assigned
        public CardComponent AvailableTypes;
    }

    public class BackgroundComponent
    {
        public string ImageId;
        public bool IsTiled;
        public GammaPalette Palette;
    }

    public class ComponentReference
    {
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public int PaddingWidth { get; internal set; }
        public int PaddingHeight { get; internal set; }

        internal void Update(int width, int height, Padding padding)
        {
            Width = width;
            Height = height;
            PaddingWidth = padding.Width;
            PaddingHeight = padding.Height;
        }
    }

    public interface ICardComponent
    {
        ComponentInfo Info { get; }
        FillInfo Fill { get; }
        void Draw(Drawable card, ref Cursor cursor, ref ComponentReference previous);
    }

    public class ImageComponent : ICardComponent
    {
        public ComponentInfo Info { get; }
        public FillInfo Fill { get; }

        public string Url;
        public int Size;
        public Color? BackgroundColor;
        public Color? FramePrimaryColor;
        public Color? FrameSecondaryColor;
        public string BorderId;

        public void Draw(Drawable card, ref Cursor cursor, ref ComponentReference previous)
        {

        }
    }

    public class TextComponent : ICardComponent
    {
        public ComponentInfo Info { get; }
        public FillInfo Fill { get; }

        public string Content;

        public FontFace Font;

        public Casing Casing;

        public Padding Padding;

        public void Draw(Drawable card, ref Cursor cursor, ref ComponentReference previous)
        {

        }
    }

    public enum FillMode
    {
        // REQ: NONE
        None = 0,

        // REQ: Palette, Primary
        Solid = 1,

        // REQ: Palette, Primary, Secondary, Direction
        Experience = 2,

        // REQ: Palette, Direction
        Gradient = 3,

        // REQ: Palette
        Reference = 4
    }

    public enum LayoutType
    {
        Default, // This is the current card layout
    }

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
                    Group = CardComponent.Avatar,
                    Priority = 0,
                    //MaxWidth = 32,
                    //MaxHeight = 32,
                    //SizeHandling = SizeHandling.Set,
                    Padding = new Padding(right: 2),
                    CursorOffset = CursorOffset.X
                },

                // Username
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardComponent.Username,
                    Priority = 1,
                    //MaxWidth = 158, // 192 - 34
                    //SizeHandling = SizeHandling.Throw,
                    Padding = new Padding(bottom: 2),
                    CursorOffset = CursorOffset.Y
                },

                // Activity
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardComponent.Activity,
                    Priority = 2,
                    Padding = new Padding(bottom: 2),
                    CursorOffset = CursorOffset.Y
                },

                // Level Icon
                new ComponentInfo
                {
                    Type = ComponentType.Icon,
                    Group = CardComponent.Level,
                    Priority = 3,
                    Padding = new Padding(right: 1),
                    CursorOffset = CursorOffset.X
                },

                // Level Counter
                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardComponent.Level,
                    Priority = 4,
                    Padding = new Padding(right: 5, bottom: 1),
                    CursorOffset = CursorOffset.None
                },

                // Exp bar
                new ComponentInfo
                {
                    Type = ComponentType.Solid,
                    Group = CardComponent.Level | CardComponent.Exp,
                    Priority = 5,
                    //MaxHeight = 2,
                    CursorOffset = CursorOffset.X,
                    OffsetHandling = OffsetHandling.Additive,
                    OffsetUsage = OffsetUsage.Include,
                    PreviousInherit = SizeInherit.Width,
                    PreviousOffsetInherit = SizeInherit.Y
                },

                new ComponentInfo
                {
                    Type = ComponentType.Icon,
                    Group = CardComponent.Money,
                    Priority = 6,
                    Padding = new Padding(right: 2),
                    CursorOffset = CursorOffset.X,
                    OffsetHandling = OffsetHandling.Additive,
                    OffsetUsage = OffsetUsage.Temporary,
                    OffsetY = -1
                },

                new ComponentInfo
                {
                    Type = ComponentType.Text,
                    Group = CardComponent.Money,
                    Priority = 7,
                    Padding = new Padding(right: 1),
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

    public class CardBuilder
    {
        public CardLayout Layout { get; set; }

    }

    public class CardInfo
    {
        public static int GetOffsetX(ComponentInfo info, Cursor cursor, ComponentReference previous)
        {
            int x = 0;

            if (info.OffsetHandling == OffsetHandling.Additive)
                x = cursor.X;

            if (info.OffsetX != 0)
                x += info.OffsetX;

            if (info.PreviousOffsetInherit.HasFlag(SizeInherit.Width))
                x += previous.Width;

            if (info.PreviousOffsetInherit.HasFlag(SizeInherit.PaddingWidth))
                x += previous.PaddingWidth;

            return x;
        }

        public static int GetOffsetY(ComponentInfo info, Cursor cursor, ComponentReference previous)
        {
            int y = 0;

            if (info.OffsetHandling == OffsetHandling.Additive)
                y = cursor.Y;

            if (info.OffsetY != 0)
                y += info.OffsetY;

            if (info.PreviousOffsetInherit.HasFlag(SizeInherit.Height))
                y += previous.Height;

            if (info.PreviousOffsetInherit.HasFlag(SizeInherit.PaddingHeight))
                y += previous.PaddingHeight;

            return y;
        }

        public bool CanTrim { get; }
        public int Width { get; }
        public int Height { get; }
        public Padding Padding { get; }
        public Padding Margin { get; }
        public int CursorOriginX { get; }
        public int CursorOriginY { get; }
        public List<ICardComponent> Components { get; }
        public GammaPalette BasePalette { get; }
        public int BorderThickness { get; }
        public BorderAllow BorderAllow { get; }
        public FillInfo BorderFill { get; }

        public string BackgroundUrl { get; }
        public bool TileBackground { get; }
        public FillInfo BackgroundFill { get; }
    }

    public class FillInfo
    {
        public Color? OutlineColor;
        public Gamma? Primary;
        public Gamma? Secondary;
        public GammaPalette Palette;
        public FillMode Usage;
        public Direction Direction;
    }

    // Get the fill space of text by simply requesting the opacity mask
    // Set up the fill design by creating a bitmap of the same width and height
}
