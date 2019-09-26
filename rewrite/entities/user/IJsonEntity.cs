using System;

namespace Orikivo
{
    public interface IJsonEntity : IEquatable<IJsonEntity>
    {
        ulong Id { get; }
        DateTime? LastSaved { get; }
        bool HasChanged { get; }
    }
}
