﻿using System;
using System.Collections.Generic;
using Orikivo;

namespace Arcadia
{
    public class TradeOffer
    {

        public static TradeOffer CloneAsOutbound(TradeOffer offer)
        {
            return new TradeOffer(offer);
        }

        private TradeOffer(TradeOffer offer, OfferType type = OfferType.Outbound)
        {
            Id = offer.Id;
            CreatedAt = offer.CreatedAt;
            Author = offer.Author;
            Target = offer.Target;
            ItemIds = offer.ItemIds;
            RequestedItemIds = offer.RequestedItemIds;
            Type = type;
        }

        public TradeOffer(Discord.IUser user, Discord.IUser target)
        {
            Author = new Author(user);
            Target = new Author(target);
            CreatedAt = DateTime.UtcNow;
            Id = KeyBuilder.Generate(5);
        }

        public string Id { get; }
        public OfferType Type { get; internal set; } = OfferType.Inbound;
        public Author Author { get; }
        public Author Target { get; }
        public DateTime CreatedAt { get; }
        public Dictionary<string, int> ItemIds { get; internal set; } = new Dictionary<string, int>();
        public Dictionary<string, int> RequestedItemIds { get; internal set; } = new Dictionary<string, int>();
    }
}