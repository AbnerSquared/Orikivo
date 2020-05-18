namespace Orikivo.Desync
{
    /// <summary>
    /// Defines all of the possible region types that can be set in a <see cref="World"/>.
    /// </summary>
    public enum RegionType
    {
        Default = 0b1,
        Barrier = 0b10,
        Structure = 0b100,
        Biome = 0b1000,
        Location = 0b10000
    }
}
