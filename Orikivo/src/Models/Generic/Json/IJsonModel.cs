using System;

namespace Orikivo.Models.Json
{
    /// <summary>
    /// Represents a generic JSON-cacheable entity.
    /// </summary>
    public interface IJsonModel : IEquatable<IJsonModel>
    {
        ulong Id { get; }
    }
}
