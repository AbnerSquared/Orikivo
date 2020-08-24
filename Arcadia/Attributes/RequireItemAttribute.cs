using System;

namespace Arcadia
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequireItemAttribute : Attribute
    {
        public RequireItemAttribute(string itemId, int amount = 1, string onFail = null)
        {
            ItemId = itemId;
            Amount = amount > 0 ? amount : 1;
            OnFail = onFail;
        }

        public string ItemId { get; }
        public int Amount { get; }
        public string OnFail { get; }
    }
}