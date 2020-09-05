using System;

namespace Orikivo.Desync
{
    // this will be used to alter attributes, statuses, and so forth
    public class Effect
    {
        // an id that points to the effect specified
        public string Id { get; }
        public float Strength { get; }
        // when the effect expires
        public DateTime? ExpiresOn { get; }
    }
}
