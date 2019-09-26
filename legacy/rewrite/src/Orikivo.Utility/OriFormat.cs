using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Orikivo.Static;

namespace Orikivo.Utility
{
    // all response formatting
    public static class OriFormat
    {
        // Returns a string in the format of an account balance.
        public static string Balance(ulong balance, ulong debt, ulong? capacity = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((balance == 0 && debt > 0) ? string.Empty : $"{EmojiIndex.Balance}{Format.Bold(balance.ToPlaceValue())} "); // balance portion
            sb.Append(debt > 0 ? $"[-{EmojiIndex.Debt}{Format.Bold(debt.ToPlaceValue())}] " : string.Empty); // debt portion
            sb.Append(capacity.HasValue ? $"• {Format.Bold(capacity.Value.ToPlaceValue())}" : string.Empty); // capacity portion
            return sb.ToString();
        }
    }
}
