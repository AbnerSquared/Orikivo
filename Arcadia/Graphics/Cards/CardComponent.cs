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
        public bool Trim;

        // This defines all of the components that this card layout will utilize
        // The card builder can then read this, and handle accordingly
        public CardComponent UsedComponents;
        // this will be used to created component presets

        // Likewise, this should also be able to understand reading component style IDs and assign them to the layout accordingly
    }
}
