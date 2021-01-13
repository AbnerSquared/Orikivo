using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Framework;
using Orikivo.Text;

namespace Arcadia
{
    public static class TradeHelper
    {
        public const string Separator = "for";
        public static readonly int MaxTargetOfferCount = 1;
        public static readonly int MaxOfferCount = 5;

        public static string ViewOffers(ArcadeUser user, ArcadeContext ctx, int page = 0)
        {
            var info = new StringBuilder();
            info.AppendLine(Locale.GetHeader(Headers.Offers));

            if (user.Offers.Count == 0)
                info.AppendLine($"\n> *You do not have any active offers.*");
            else
            {
                var offers = new List<TradeOffer>(user.Offers);
                foreach (TradeOffer offer in offers)
                {
                    info.AppendLine($"\n{ViewOffer(offer, ctx)}");
                }
            }

            return info.ToString();
        }

        public static string DeclineOffer(ArcadeUser user, ArcadeUser author, TradeOffer offer)
        {
            if (offer.Author.Id != author.Id)
                throw new Exception("Expected author to match offer author");

            user.Offers.RemoveAll(x => x.Id == offer.Id);
            author.Offers.RemoveAll(x => x.Id == offer.Id);

            return $"Successfully {(offer.Type == OfferType.Inbound ? "declined" : "canceled the pending")} offer `{offer.Id}`{(offer.Type == OfferType.Inbound ? $"from **{author.Username}**" : "")}.";
        }

        public static string AcceptOffer(ArcadeUser target, ArcadeUser author, TradeOffer offer)
        {
            // Get the author in the command
            if (offer.Author.Id != author.Id)
                throw new Exception("Expected author to match offer author");

            if (offer.Type == OfferType.Outbound)
                return Format.Warning("You cannot accept outbound trade offers.");

            if (!IsOfferValid(author, target, offer))
            {
                author.Offers.RemoveAll(x => x.Id == offer.Id);
                target.Offers.RemoveAll(x => x.Id == offer.Id);
                return Format.Warning("This trade offer has expired or is invalid due to missing items.");
            }

            foreach ((string itemId, int amount) in offer.ItemIds)
            {
                ItemHelper.TakeItem(author, itemId, amount);
                ItemHelper.GiveItem(target, itemId, amount);
            }

            foreach ((string itemId, int amount) in offer.RequestedItemIds)
            {
                ItemHelper.TakeItem(target, itemId, amount);
                ItemHelper.GiveItem(author, itemId, amount);
            }

            target.Offers.RemoveAll(x => x.Id == offer.Id);
            author.Offers.RemoveAll(x => x.Id == offer.Id);

            if (author.Config.CanNotify(NotifyAllow.OfferAccepted))
            {
                author.Notifier.Add($"Your trade offer (`{offer.Id}`) has been accepted.");
            }

            return $"Successfully accepted offer `{offer.Id}` from **{offer.Author.ToString("Unknown User")}**.{(offer.ItemIds.Count > 0 ? $"\nYou have received:\n{string.Join("\n", offer.ItemIds.Select(x => WriteItem(x.Key, x.Value)))}" : "")}";
        }

        private static string WriteItem(string itemId, int amount)
        {
            Item item = ItemHelper.GetItem(itemId);
            string icon = ItemHelper.GetIconOrDefault(itemId);
            string name = Check.NotNull(icon) ? item.Name : item.GetName();
            return $"{(Check.NotNull(icon) ? $"{icon} " : "• ")}**{name}**{(amount > 1 ? $" (x**{amount:##,0}**)" : "")}";
        }

        public static bool IsOfferValid(ArcadeUser author, ArcadeUser target, TradeOffer offer)
        {
            if ((DateTime.UtcNow - offer.CreatedAt) >= TimeSpan.FromHours(24))
                return false;

            foreach ((string itemId, int amount) in offer.ItemIds)
            {
                if (ItemHelper.GetOwnedAmount(author, itemId) < amount)
                    return false;
            }

            foreach ((string itemId, int amount) in offer.RequestedItemIds)
            {
                if (ItemHelper.GetOwnedAmount(target, itemId) < amount)
                    return false;
            }

            return true;
        }

        public static string SendOffer(ArcadeUser author, ArcadeUser target, TradeOffer offer)
        {
            if (author.Offers.Count(x => x.Target.Id == target.Id) >= MaxTargetOfferCount)
                return Format.Warning($"You already have an active trade offer to **{target.Username}**. Please cancel your current offer to this user before sending a new one.");

            if (author.Offers.Count >= MaxOfferCount)
                return Format.Warning("You already have too many active trade offers. Try again later.");

            foreach ((string itemId, int amount) in offer.ItemIds)
            {
                if (ItemHelper.GetOwnedAmount(author, itemId) < amount)
                    return Format.Warning("You do not own one of the specified items in your trade offer.");
            }

            foreach ((string itemId, int amount) in offer.RequestedItemIds)
            {
                if (ItemHelper.GetOwnedAmount(target, itemId) < amount)
                    return Format.Warning($"**{target.Username}** does not own one of the specified items in your trade offer.");
            }

            if (target.Offers.Count == MaxOfferCount)
                return Format.Warning($"**{target.Username}** already has too many pending trade offers. Try again later.");

            target.Offers.Add(offer);
            author.Offers.Add(TradeOffer.CloneAsOutbound(offer));

            if (target.Config.CanNotify(NotifyAllow.OfferInbound))
            {
                target.Notifier.Add("You have received a new trade offer.");
            }

            return $"Successfully sent **{target.Username}** a trade offer.";
        }

        public static string ViewOffer(TradeOffer offer, ArcadeContext ctx)
        {
            var info = new StringBuilder();

            if (!offer.Target.Id.HasValue || !offer.Author.Id.HasValue)
                throw new Exception("Expected a specified user ID for both the target and author in the specified trade offer");

            ctx.TryGetUser(offer.Target.Id.Value, out ArcadeUser target);
            ctx.TryGetUser(offer.Author.Id.Value, out ArcadeUser author);

            if (!IsOfferValid(author, target, offer))
            {
                info.AppendLine(Format.Warning("This trade offer is invalid or expired and will be removed."));
                target.Offers.RemoveAll(x => x.Id == offer.Id);
                author.Offers.RemoveAll(x => x.Id == offer.Id);
            }

            info.AppendLine($"> `{offer.Id}` {(offer.Type == OfferType.Inbound ? $"**From: {offer.Author.ToString("Unknown User")}**" : $"**To: {offer.Target.ToString("Unknown User")}**")}");
            info.AppendLine($"> Expires in {Format.Counter(TimeSpan.FromHours(24) - (DateTime.UtcNow - offer.CreatedAt))}");
            if (offer.ItemIds.Count > 0)
            {
                info.AppendLine("**Offers**:");
                info.AppendJoin("\n", offer.ItemIds.Select(x => WriteItem(x.Key, x.Value)));

                if (offer.RequestedItemIds.Count > 0)
                    info.AppendLine();
            }

            if (offer.RequestedItemIds.Count > 0)
            {
                info.AppendLine("**Requests**:");
                info.AppendJoin("\n", offer.RequestedItemIds.Select(x => WriteItem(x.Key, x.Value)));
            }

            return info.ToString();
        }

        // TODO: Make the base trade offer parsing generic, so that it can be used for other purposes (such as using items)
        public static bool TryParseOffer(Discord.IUser user, Discord.IUser target, string input, out TradeOffer offer)
        {
            offer = null;

            input = input.Trim();
            var reader = new StringReader(input);

            // item,amount item,amount item

            bool requested = false;
            bool remainder = false;

            if (!reader.CanRead())
                return false; // throw new Exception("Expected a string to read but returned null");

            var offered = new Dictionary<string, int>();
            var requests = new Dictionary<string, int>();

            while (reader.CanRead())
            {
                string arg;
                int amount = 1;

                if (!reader.GetRemaining().Contains(' '))
                {
                    arg = reader.GetRemaining();
                    remainder = true;
                }
                else
                {
                    arg = reader.ReadUntil(' ');
                }

                if (arg == Separator)
                {
                    if (requested)
                        return false; // throw new Exception("Offer separator has already been specified");

                    requested = true;
                    continue;
                }

                if (arg.Contains(','))
                {
                    if (arg.Count(x => x == ',') > 1)
                        return false; // throw new Exception("Expected a single comma separator");

                    int.TryParse(arg[arg.IndexOf(',')..], out amount);

                    if (amount <= 0)
                        amount = 1;

                    arg = arg.Substring(0, arg.Length - (arg.Length - (arg.IndexOf(',') + 1)));
                }

                if (!ItemHelper.Exists(arg))
                    return false; // throw new Exception("Expected valid item ID");

                if (requested)
                {
                    requests.Add(arg, amount);
                }
                else
                {
                    offered.Add(arg, amount);
                }

                if (remainder)
                    break;
            }

            offer = new TradeOffer(user, target)
            {
                CreatedAt = DateTime.UtcNow,
                ItemIds = offered,
                RequestedItemIds = requests
            };

            return true;
        }
    }
}
