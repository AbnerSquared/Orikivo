﻿namespace Arcadia
{
    /// <summary>
    /// Represents a base component for a <see cref="Recipe"/>.
    /// </summary>
    public class RecipeComponent
    {
        public RecipeComponent(string itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }

        // bring in the idea of referencing any item in a group instead
        public string ItemId { get; set; }

        public int Amount { get; set; }

        internal void Deconstruct(out string itemId, out int amount)
        {
            itemId = ItemId;
            amount = Amount;
        }
    }
}
