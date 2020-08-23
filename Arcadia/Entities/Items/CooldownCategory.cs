namespace Arcadia
{
    public enum CooldownCategory
    {
        Instance = 1, // Applied to this unique instance only
        Item = 2, // Applied to this item only
        Group = 3, // Applied to this item's group only (throw an exception if no group is specified
        Global = 4 // Applied to all items as a whole
    }
}