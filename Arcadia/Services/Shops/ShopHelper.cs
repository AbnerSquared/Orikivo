using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    public static class ShopHelper
    {
        public static readonly List<Shop> Shops =
            new List<Shop>
            {

            };

        public static Shop GetShop(string id)
        {
            if (Shops.Count(x => x.Id == id) > 1)
                throw new ArgumentException("There are more than one shops with the specified ID.");

            return Shops.FirstOrDefault(x => x.Id == id);
        }

        public static string NameOf(string shopId)
            => GetShop(shopId).Name;
    }
}