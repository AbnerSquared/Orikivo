namespace Orikivo
{
    // this combines the dynamic and hardformat info together
    public struct OriItem
    {
        //public OriItem FromId(string id)
        //{
        //
        //}

        public ulong Value { get; }
        public bool CanUse { get; }
        public bool CanTrade { get; }
        public bool CanBuy { get; }
        public bool CanSell { get; }
        public bool CanGift { get; }
        public int? TradesLeft { get; }
        public int? GiftsLeft { get; }
        public int? UsesLeft { get; }
        public ItemGroupType GroupType { get; }
        public ItemCustomAction? ActionType { get; }
        public double? CooldownLength { get; }
    }
}
