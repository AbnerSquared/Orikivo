namespace Orikivo
{
    /// <summary>
    /// Defines all of the possible location types that can be accessed in a <see cref="Desync.World"/>.
    /// </summary>
    [System.Flags]
    public enum LocationType
    {
        Sector = 1,
        Field = 2,
        Area = 4,
        Construct = 8
    }
}
