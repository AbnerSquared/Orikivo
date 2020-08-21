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

        public string Name { get; set; }

        public List<string> OnEnter { get; set; }

        public List<string> OnMenu { get; set; }

        public List<string> OnTimeout { get; set; }

        public List<string> OnBuy { get; set; }

        public List<string> OnSell { get; set; }

        public List<string> OnViewBuy { get; set; }

        public List<string> OnViewSell { get; set; }

        public List<string> OnExit { get; set; }
    }
}