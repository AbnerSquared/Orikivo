using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic JSON-cacheable entity.
    /// </summary>
    public interface IJsonEntity : IEquatable<IJsonEntity>
    {
        ulong Id { get; }
        DateTime? LastSaved { get; }
    }
}
