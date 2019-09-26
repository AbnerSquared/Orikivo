namespace Orikivo
{
    /// <summary>
    /// Defines the scarcity of an Item.
    /// </summary>
    public enum Rarity
    {
        Abundant = 1, // extremely common
        Common = 2, // common
        Uncommon = 4, // fairly common
        Rare = 8, // extremely uncommon
        Scarce = 16, // rare
        Unheard = 32, // very rare
        Mythical = 64, // insane rarity
        Nullified = 128 // items only a selective few hold.
    }
}
