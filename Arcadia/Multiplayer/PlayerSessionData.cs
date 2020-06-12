using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    public class PlayerSessionData
    {
        public Player Player { get; internal set; }

        public List<GameProperty> Attributes { get; set; }

        public void SetAttribute(string id, object value)
        {
            if (!Attributes.Any(x => x.Id == id))
                throw new System.Exception($"Unable to find the specified attribute '{id}'");

            Attributes.First(x => x.Id == id).Set(value);
        }

        public GameProperty GetAttribute(string id)
        {
            if (!Attributes.Any(x => x.Id == id))
                throw new System.Exception($"Unable to find the specified attribute '{id}'");

            return Attributes.First(x => x.Id == id);
        }
    }

}
