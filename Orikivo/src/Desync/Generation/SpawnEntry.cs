namespace Orikivo.Desync
{
    public class SpawnEntry : ITableEntry
    {
        public string CreatureId { get; set; }
        public CreatureTag Groups { get; set; }
        public int Weight { get; set; }
    }
}
