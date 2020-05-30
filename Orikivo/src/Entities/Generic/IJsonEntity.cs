using System;

namespace Orikivo
{
    // Barebones class
    /// <summary>
    /// Represents a generic JSON-cacheable entity.
    /// </summary>
    public interface IJsonEntity : IEquatable<IJsonEntity>
    {
        ulong Id { get; }
    }
}
