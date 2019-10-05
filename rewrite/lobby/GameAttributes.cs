using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class GameAttributes
    {
        public GameAttributes(List<GameAttribute> attributes)
        {
            Attributes = attributes;
        }
        public List<GameAttribute> Attributes { get; }
        public bool Update(GameAttributeUpdate update)
        {
            if (!Attributes.Any(x => x.Name == update.Id))
                throw new Exception("The update packet is trying to update an attribute that doesn't exist.");
            Attributes.First(x => x.Name == update.Id).Value += update.Amount;
            return true;
        }
    }
}
