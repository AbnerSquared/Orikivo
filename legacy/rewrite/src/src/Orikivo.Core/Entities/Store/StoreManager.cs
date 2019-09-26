using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System;

namespace Orikivo
{
    // fix this broken mess of a shop system..?

    public enum ActionType
    {
        ClearDebt
    }

    public class ActionItem
    {
        public ActionItem(string name, ulong cost, string description, ActionType action)
        {
            Name = name ?? "new-action-item";
            Cost = cost;
            Description = description ?? "default-description";
            Action = action;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public ulong Cost { get; set; }
        public ActionType Action { get; set; }

        public void Invoke(OldAccount a, OrikivoCommandContext Context)
        {
            DoAction(a, Context);
        }

        private void DoAction(OldAccount a, OrikivoCommandContext Context)
        {
            switch(Action)
            {
                case ActionType.ClearDebt:
                    ClearDebt(a, Context);
                    break;
            }
        }

        private void ClearDebt(OldAccount a, OrikivoCommandContext Context)
        {
            Console.WriteLine("I AM A POCKET LAWYER AND IM HERE TO HELP");
            OldMail m = new OldMail("Mr. Pocket", "I'm Here to Help", new CompactMessage("I have contacted ORS, and they have agreed to let me clear your debt. I hope that you'll do better next time."));
            m.SendAsync(a, Context.Client);
            Context.Data.Update(a);
            a.ClearDebt();
            a.Save();
        }
    }

    public class StoreManager
    {

        public StoreManager()
        {
            SchemeIndex schemes = new SchemeIndex(); // the color scheme data.
            ColorSchemes = new List<OldCardColorScheme>();
            //ColorSchemes.Add(schemes.Baseline);
            ColorSchemes.Add(schemes.OriGreen);
            ColorSchemes.Add(schemes.FadingRed);
            ColorSchemes.Add(schemes.Wumpunium);

            Consumables = new List<ActionItem>();

            ActionItem pocketLawyer = new ActionItem("Pocket Lawyer", 40, "Helps you when getting out of that tough debt.", ActionType.ClearDebt);

            Consumables.Add(pocketLawyer);
        }

        

        // a system for holding items, having daily specials, selling, buying, gifting
        // need to make a class hold colorscheme items
        public List<OldCardColorScheme> ColorSchemes { get; set; } // a list of color schemes available for purchase.
        public List<ActionItem> Consumables { get; set; }
        // buys the name of a scheme
        public void BuyConsumable(OldAccount a, string name, out ActionItem purchase)
        {
            purchase = null;
            ActionItem item = Consumables.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (item == null)
                return;

            if (a.TryBuy(item))
            {
                a.Store(item);
                purchase = item;
                return;
            }
        }

        public void BuyScheme(OldAccount a, string name, out OldCardColorScheme purchase)
        {
            purchase = null;
            OldCardColorScheme item = ColorSchemes.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

            if (item == null)
                return;

            if (a.TryBuy(item))
            {
                a.Store(item);
                purchase = item;
                return;
            }
        }
    }
}