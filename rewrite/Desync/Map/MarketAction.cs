namespace Orikivo.Unstable
{
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
