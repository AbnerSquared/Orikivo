namespace Orikivo
{
    // The base class of a card.
    public class Card
    {
        // Card Config
        public int Scale { get; set; } // How big would you want this card to render?
        public CardBorder Border { get; set; }
        public CardBackground Backdrop { get; set; }
    }

    public class CardComponent
    {
        // ComponentType - Is it an avatar, experience bar...?
        // ComponentConfig - What fancy tweaks did you buy to make this look nicer?
        // Priority - How important is it that this specific thing gets rendered?
    }

    public class CardLayout
    {
        // this defines where all objects should be rendered
        // at what? point
    }
}