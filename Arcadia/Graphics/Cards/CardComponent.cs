using System;
using Discord;
using Orikivo.Drawing;

namespace Arcadia.Graphics
{
    public enum CardComponent
    {
        Username = 1,
        Activity = 2,
        Border = 3,
        Background = 4,
        Avatar = 5,
        Level = 6,
        Money = 7,
        Exp = 8, // foreground exp
        Bar = 9 // background exp
    }

    public enum ComponentType
    {
        Image = 1,
        Icon = 2,
        Text = 3,
        Bar = 4
    }

    public interface ICardComponent
    {
        public ComponentType Type { get; }
        public GammaPalette Palette { get; }
        public int OffsetX { get; }
        public int OffsetY { get; }
    }

    public class ImageComponent
    {
        public string Url;
        public int Size;
        public Color? BackgroundColor;
        public Color? FramePrimaryColor;
        public Color? FrameSecondaryColor;
        public string BorderId;
        public GammaPalette ImagePalette;
        public Padding Padding;
        public int OffsetX;
        public int OffsetY;
    }

    public class BorderComponent
    {
        public int Width;
        public BorderAllow Sides;
        public string FillStyleId;
        public Padding Padding;
        public Padding Margin;
        public Color? PrimaryColor;
        public Color? SecondaryColor;
    }

    public class TextComponent
    {
        public string Content;
        public FontFace Font;
        public Casing Casing;
        public Color? OutlineColor;
        public Gamma? PrimaryGamma;
        public Gamma? SecondaryGamma;
        public GammaPalette Palette; // If the CardLayout specifies that the text will not use primary/secondary, it will instead be ignored
        public string FillStyleId; // solid, based on EXP, gradient, etc.
        public Shadow Shadow; // an optional text shadow
        public int OffsetX;
        public int OffsetY;
        public Padding Padding;
    }

    public enum FillType
    {
        Solid,
        Exp
    }

    public class FillStyle
    {
        // This can handle custom fill styles
    }

    // Likewise, this is also handled by Merit slots
    public class IconComponent
    {
        public Sheet SheetReference;
        public int SheetIndex;
        public GammaPalette Palette;
        public FrameComponent Frame;
        public int OffsetX;
        public int OffsetY;
    }

    public class FrameComponent
    {
        public Sheet SheetReference;
        public int SheetIndex;
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

    // This incorporates: CardLayout, CardConfig (will be renamed to CardProperties), CardDetails, to create all of the proper card components
    public class CardBuilder
    {
        public BackgroundComponent Background;
    }


    public class Card
    {
        // This gathers up all of the compiled/finalized components to draw in the GraphicsService
    }

    public enum LayoutType
    {
        Default, // This is the current card layout
    }

    public class CardLayout
    {
        public const int MaxImageWidth = 400;
        public const int MaxImageHeight = 300;

        public static readonly CardLayout Default = new CardLayout
        {
            // GraphicsService should draw each component in increasing priority
            /* CardLayout.Default
             * Width = 192
             * Height = 32
             * Padding = 2
             * Margin = 2
             * Origin = (0, 0)
             * CanTrim = true [Do not trim the card if it cannot be trimmed]
             *
             * Border
             * Thickness = 2
             *
             * Avatar
             * Type = ComponentType.Image
             * Priority = 0
             * Width = 32
             * Height = 32
             * Padding = Right: 2
             * Margin = 0
             * CursorOffset = CursorOffset.X
             *
             * Name
             * Type = ComponentType.Text
             * Priority = 1
             * Width = Font.WidthOf(Details.Name)
             * Height = Font.CharHeight
             * Padding = Bottom: 2
             * CursorOffset = CursorOffset.Y
             *
             * Activity
             * Type = ComponentType.Text
             * Priority = 2
             * Width = Font.WidthOf(Details.Activity)
             * Height = Font.CharHeight
             * Padding = Bottom: 2
             * CursorOffset = CursorOffset.Y
             *
             * Level/ (Group)
             *
             * Icon
             * Type = ComponentType.Icon
             * Priority = 3
             * Reference = @"../assets/icons/levels.png"
             * ReferencePointer = (1, 1)
             * Width = 6
             * Height = 6
             * Padding = Right: 1
             * CursorOffset = CursorOffset.X
             * 
             * Counter
             * Priority = 4
             * Type = ComponentType.Counter
             * Width = Font.WidthOf(Details.Level)
             * Height = Font.CharHeight
             * Padding = Right: 5, Bottom: 1
             * CursorOffset = CursorOffset.None
             * CanShowSuffix = false
             *
             * Exp
             * Type = ComponentType.Bar
             * Priority = 5
             * Width = Counter.Width
             * Height = 2
             * OffsetY = Counter.Height + Counter.Padding.Height
             * OffsetHandling = OffsetHandling.Additive
             * CursorOffset = CursorOffset.X
             *
             * Money/ (Group)
             *
             * Icon
             * Type = ComponentType.Icon
             * Priority = 6
             * Reference = @"../assets/icons/coins.png"
             * ReferencePointer = (1, 1)
             * Width = 8
             * Height = 8
             * Padding = Right: 2
             * CursorOffset = CursorOffset.X
             * CursorOffsetHandling = OffsetHandling.Additive
             * CursorOffsetUsage = OffsetUsage.Temporary
             * CursorOffsetY = -1 (temporary set the cursor offset of y -1, then place it back once done)
             *
             * Counter
             * Type = ComponentType.Counter
             * Priority = 7
             * Width = Font.WidthOf(text)
             * Height = Font.CharHeight
             * Padding = Right: 1
             * CursorOffset = CursorOffset.X
             * CanShowSuffix = true
             * MaxLength = 3
             *
             * Suffix
             * Type = ComponentType.Icon
             * Priority = 8
             * Reference = @"../assets/icons/suffixes.png"
             * ReferencePointer = (1, NumberGroup)
             * Width = 6
             * Height = 6
             * CursorOffset = CursorOffset.None
             */
        };

        public bool Trim;

        // This defines all of the components that this card layout will utilize
        // The card builder can then read this, and handle accordingly
        public CardComponent UsedComponents;
        // this will be used to created component presets

        // Likewise, this should also be able to understand reading component style IDs and assign them to the layout accordingly
    }

    public enum OffsetUsage
    {
        // The cursor offset specified will be reverted after it is drawn
        Temporary = 1,

        // The cursor offset specified will not be reverted
        Include = 2
    }

    public interface IComponentInfo
    {
        public ComponentType Type { get; }
        public int Priority { get; }
        public int Width { get; }
        public int Height { get; }

        public Padding Padding { get; }
    }

    public class IconComponentInfo : IComponentInfo
    {
        public ComponentType Type => ComponentType.Icon;
        public int Priority { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public Padding Padding { get; set; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int CursorOffsetX { get; set; }
        public int CursorOffsetY { get; set; }

        public OffsetHandling CursorOffsetHandling { get; set; }
        public CursorOffset CursorOffset { get; set; }
        public OffsetUsage CursorOffsetUsage { get; set; }
        public OffsetUsage OffsetUsage { get; set; }
        public OffsetHandling OffsetHandling { get; set; }
    }

    public class CounterComponent
    {

    }

    [Flags]
    public enum CursorOffset
    {
        None = 0,
        X = 1,
        Y = 2,
        XY = X | Y
    }

    public enum OffsetHandling
    {
        // The specified Offset will replace the cursor
        Replace = 1,

        // The specified offset will be conjoined with the cursor
        Additive = 2
    }
}
