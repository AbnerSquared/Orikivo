using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic JSON-cacheable entity.
    /// </summary>
    public interface IJsonModel : IEquatable<IJsonModel>
    {
        ulong Id { get; }
    }
}
