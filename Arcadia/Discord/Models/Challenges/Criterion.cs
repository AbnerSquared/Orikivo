using System;
using Arcadia.Multiplayer;

namespace Arcadia
{
    /// <summary>
    /// Represents a basic requirement.
    /// </summary>
    public class Criterion
    {
        public string Id { get; set; }
        public Func<CriterionContext, bool> Judge { get; set; }
    }

    /*
    public abstract class Criterion
    {
        public string Id { get; protected set; }

        public abstract bool Judge(ArcadeUser user, object callback);
    }
    */
}