namespace Orikivo.Unstable
{
    public enum MarketState
    {
        Menu, // in main menu
        Sell, // in sell menu
        SellConfirm, // multiple counter thing
        SellComplete, // sold an item
        Buy, // in buy menu
        BuyConfirm, // multiple counter thing
        BuyComplete, // bought an item
        SellRevert, // bought item back
        View // inspecting item
    }

    public enum MarketAction
    {
        Enter, // entering the market
        Exit, // leaving the market
        Sell, // selling an item
        SellAll, // selling everything
        SellCancel, // cancelling a sell
        Buy, // buying an item
        BuyAll, // buying everything
        BuyCancel, // cancelling a buy
        View, // inspecting an item
        Catalog // referring to the store's catalog
    }
}
