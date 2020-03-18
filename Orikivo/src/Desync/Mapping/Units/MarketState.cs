namespace Orikivo.Desync
{
    /// <summary>
    /// Defines a specific state a <see cref="Husk"/> is doing at a <see cref="Market"/>.
    /// </summary>
    public enum MarketState
    {
        Menu, // in main menu
        Sell, // in sell menu
        SellConfirm, // multiple counter thing
        // SellAll, // sold everything on hand
        SellComplete, // sold an item
        // SellCancel, // cancelled a sell
        Buy, // in buy menu
        BuyConfirm, // multiple counter thing
        // BuyAll, // bought everything on hand
        BuyComplete, // bought an item
        // BuyCancel, // cancelled a purchase
        SellRevert, // bought item back
        View // inspecting item
    }
}
