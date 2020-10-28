namespace Arcadia
{
    public enum TradeState
    {
        Invite, // This is the initial state for trading, at which a user is waiting for the other user to accept
        Menu, // The menu showing the items that will be traded between two users.
        Inventory, // The inventory display screen, waiting for a user to select their item.
        Item, // The item select screen, waiting for a user to select an item with amount
        Success, // The trade has gone through
        Cancel // The trade has been cancelled
    }
}
