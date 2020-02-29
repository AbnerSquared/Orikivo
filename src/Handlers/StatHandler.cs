using Discord;
using System.Collections.Generic;

namespace Orikivo
{
    internal static class StatHandler
    {
        internal static string GetNameOrDefault(string stat)
        {
            Dictionary<string, string> statNames = new Dictionary<string, string>
            { ["times_cried"] = "Times Cried" };

            return statNames.ContainsKey(stat) ? Format.Bold(statNames[stat]) : $"`{stat}`";
        }
    }
}