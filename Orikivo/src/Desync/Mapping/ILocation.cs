using System.Collections.Generic;

namespace Orikivo.Desync
{
    public interface ILocation
    {
        string Id { get; }
        string Name { get; }
        LocationType Type { get; }
        float Longitude { get; }
        float Latitude { get; }
        ILocation Parent { get; }
        IEnumerable<ILocation> Children { get; }
    }
}
