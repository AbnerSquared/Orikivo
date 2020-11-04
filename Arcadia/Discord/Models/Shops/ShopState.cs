namespace Arcadia
{
    // TODO: Simplify primary shop state to their rudimentary states, and create a secondary sub-state.
    // Here's an example:
    // ShopFlags.Empty | ShopFlags.Deny | ShopFlags.Invalid | ShopFlags.Forbidden | ShopFlags.NotOwned | ShopFlags.Remainder | ShopFlags.Limited
    public enum ShopState
    {
        Enter = 1,
        Menu = 2,
        Buy = 3,
        Sell = 4,
        Timeout = 5,
        ViewBuy = 6,
        ViewSell = 7,
        Exit = 8,
        BuyDeny = 9,
        BuyEmpty = 10,
        BuyFail = 11,
        SellDeny = 12,
        SellEmpty = 13,
        BuyInvalid = 14,
        SellInvalid = 15,
        SellNotAllowed = 16,
        SellNotOwned = 17,
        BuyRemainder = 18,
        BuyLimit = 19
    }
}
