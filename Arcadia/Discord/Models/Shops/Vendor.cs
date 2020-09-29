using System.Collections.Generic;

namespace Arcadia
{
    public class Vendor
    {
        public static readonly string NameGeneric = "Vendor";
        public static readonly string EnterGeneric = "Welcome.";
        public static readonly string MenuGeneric = "What can I do for you?";
        public static readonly string TimeoutGeneric = "I'm busy right now. Come back later.";
        public static readonly string BuyGeneric = "Thank you for your business.";
        public static readonly string SellGeneric = "Thank you for the sale.";
        public static readonly string ViewBuyGeneric = "This is everything I have for sale.";
        public static readonly string ViewSellGeneric = "What can I buy from you?";
        public static readonly string ExitGeneric = "Goodbye.";
        public static readonly string BuyDenyGeneric = "I don't sell items.";
        public static readonly string SellDenyGeneric = "I don't buy items.";
        public static readonly string BuyEmptyGeneric = "I don't have anything else I can offer.";
        public static readonly string BuyFailGeneric = "You can't afford to pay for all of this.";
        public static readonly string SellEmptyGeneric = "You don't have anything I can buy from you.";
        public static readonly string BuyInvalidGeneric = "I can't figure out what item you are trying to purchase.";
        public static readonly string SellInvalidGeneric = "I'm not sure what item you want me to buy from you.";
        public static readonly string InvalidAmountGeneric = "You have to at least specify 1 at a minimum.";
        public static readonly string SellNotOwnedGeneric = "You don't have this item in your inventory.";
        public static readonly string SellNotAllowedGeneric = "Sorry, but I don't buy that here.";
        public static readonly string BuyRemainderGeneric = "Thank you for buying everything in stock.";
        public static readonly string BuyLimitGeneric = "You have reached your purchase limit.";

        public string Name { get; set; }

        public ItemTag PreferredTag { get; set; }

        public string[] OnEnter { get; set; }

        public string[] OnMenu { get; set; }

        public string[] OnTimeout { get; set; }

        public string[] OnBuy { get; set; }
        public string[] OnBuyRemainder { get; set; }
        public string[] OnBuyDeny { get; set; }

        public string[] OnBuyFail { get; set; }

        public string[] OnBuyEmpty { get; set; }
        public string[] OnBuyInvalid { get; set; }
        public string[] OnBuyLimit { get; set; }

        public string[] OnSell { get; set; }
        public string[] OnSellDeny { get; set; }
        public string[] OnSellEmpty { get; set; }
        public string[] OnSellInvalid { get; set; }
        public string[] OnSellNotAllowed { get; set; }
        public string[] OnSellNotOwned { get; set; }


        public string[] OnViewBuy { get; set; }

        public string[] OnViewSell { get; set; }

        public string[] OnExit { get; set; }
    }
}