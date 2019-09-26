using Orikivo.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents as a helper class for insults.
    /// </summary>
    public class SledgeHelper
    {
        private string GetAny(string[] strings)
            => strings[RandomProvider.Instance.Next(1, strings.Length + 1) - 1];

        public string GetAnyInsult(SledgePower sp)
            => GetAny(GetArray(sp));

        public string[] GetArray(SledgePower sp)
        {
            string[] strings = { };
            return strings;
        }
    }
}
