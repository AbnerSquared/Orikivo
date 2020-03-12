namespace Orikivo.Desync
{
    /// <summary>
    /// Defines all of the possible location types that can be accessed in a <see cref="Desync.World"/>.
    /// </summary>
    [System.Flags]
    public enum LocationType
    {
        World = 0b1,
        Field = 0b10,
        Sector = 0b100,
        Area = 0b1000,
        Construct = 0b10000,
        Structure = 0b100000
    }
}
