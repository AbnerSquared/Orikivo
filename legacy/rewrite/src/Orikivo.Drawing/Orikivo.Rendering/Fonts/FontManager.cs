using Orikivo.Storage;
using System.Linq;

namespace Orikivo
{
    public static class FontManager
    {
        public static FontCache FontMap { get; private set; }

        static FontManager()
        {
            FontMap = FileManager.GetFonts();
        }
    }

    public static class ShopManager
    {
        public static ShopCache ShopMap { get; private set; }

        static ShopManager()
        {
            ShopMap = FileManager.GetShops();
        }
    }

    public static class ItemManager
    {
        public static ItemCache ItemMap { get; private set; }

        static ItemManager()
        {
            ItemMap = FileManager.GetItems();
        }

        public static bool HasGroup(ushort id)
            => ItemMap.Items.Any(x => (ushort)x.Group == id);
    }
}