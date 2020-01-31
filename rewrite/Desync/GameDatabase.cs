using System;
using System.Collections.Generic;

namespace Orikivo.Unstable
{

    public static class GameDatabase
    {
        public static World World => new World { };
        // TODO: Implement world design. World => Field => Sector => Area => Construct
        //     Fields are wildlands. Sectors reside within a field, as some sort of safe zone.
        //     Areas are small gatherings within sectors. Constructs are a building within an area.
        public static Dictionary<string, Item> Items => new Dictionary<string, Item>
        {
            ["pocket_lawyer"] = new Item
            {
                Id = "pocket_lawyer",
                Name = "Pocket Lawyer",
                Summary = "Constantly puts up with ORS to keep you safe.",
                Quotes = new List<string>
                {
                    "You'll get the chance to dispute in court, you'll see.",
                    "ORS doesn't stand a chance." },
                BypassCriteriaOnGift = true,
                GiftLimit = 1,
                Rarity = ItemRarity.Common,
                Tag = ItemTag.Callable,
                ToOwn = u => u.Debt >= 1000,
                Value = 40,
                CanBuy = true,
                CanSell = false,
                Action = new ItemAction
                {
                    UseLimit = 1,
                    BreakOnLastUse = true,
                    Cooldown = TimeSpan.FromHours(24),
                    OnUse = u => u.Debt = 0 }
            }
        };

        public static Dictionary<string, Merit> Merits => new Dictionary<string, Merit>
        {
            ["test"] = new Merit { Criteria = x => x.GetStat("times_cried") == 1,
                Name = "Shedding Tears", Summary = "Cry.", Group = MeritGroup.Misc, Reward = new Reward { Money = 10 } }
        };

        public static Dictionary<string, Claimable> Claimables => new Dictionary<string, Claimable>();

        public static Dictionary<string, Booster> Boosters => new Dictionary<string, Booster>();

        public static Claimable GetClaimable(string id)
            => Claimables[id];

        public static Merit GetMerit(string id)
            => Merits[id];

        public static Booster GetBooster(string id)
            => Boosters[id];
        

        public static Item GetItem(string name)
            => Items[name];
        
    }

    // loot is stuff like:
    // - sockets, which enhance your digital features
    // - new backpacks to store more stuff at a time
    // - transportation devices, which speed up travel time between places
    // - collectables in the real world, which can be traded for valuables in the digital
    // - CardTech, which enhances and modifies how your card is displayed
    //   CardTech items: // By default, your card color scheme is bound to your interface color scheme, unless you explicitly modify it
    //   - Color Schemes (Bind IDs to GammaColorMap)
    //   - Font faces (Bind IDs to FontFace)
    //   - Avatar animator (Simply a flag)
    //   - New layout templates, which change how your card is organized. (you can unlock an advanced card layout concept, which allows you to set your card to literally anything
    //   - Avatar canvas, which allows you to instead draw your avatar
    //   - Avatar size regulator, which allows you to modify the avatar appearance size on your card.
    //   - Status Templates, which change how your status is displayed color-wise
    //   - Level Templates, which change how your level is displayed
    //   - Exp Templates, which change how your current experience is displayed (exp template can be set referencing another template)
    //   - Username templates, which change how your name is displayed
    //   - Font Face Modifiers, which change how the font is written
    //   - Backgrounds, which are displayed behind a card.
    //   - Merit Slots, which allow you to display the icon of a merit onto a card


    // - Interface Enhancements, which modifies how Orikivo visually displays content to you
}
