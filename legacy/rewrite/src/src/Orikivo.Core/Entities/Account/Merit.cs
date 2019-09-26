using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// Represents an accolade on Orikivo.
    /// </summary>
    public class OldMerit
    {
        [JsonConstructor]
        public OldMerit(int id, string name, string flavorText, string path)
        {
            Id = id;
            Name = name;
            FlavorText = flavorText;
            Path = path;
        }
        
        /// <summary>
        /// The identifier used to sort merit creation date.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The name displayed on the Merit.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The flavor text describing the Merit.
        /// </summary>
        public string FlavorText { get; }

        /// <summary>
        /// The local resource file path leading to the image of the Merit.
        /// </summary>
        public string Path { get; }
    }

    /// <summary>
    /// Represents the collection of merits that are referenced upon launch.
    /// </summary>
    public class MeritCache
    {
        public MeritCache(List<Merit> merits)
        {
            Merits = merits;
        }
        /// <summary>
        /// The collection of available merits found from the resource path.
        /// </summary>
        public List<Merit> Merits { get; }

        internal void ForEach(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}