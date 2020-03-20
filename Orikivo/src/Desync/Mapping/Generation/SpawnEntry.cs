namespace Orikivo.Desync
{
    public class SpawnEntry : ITableEntry
    {
        public string CreatureId { get; set; }
        public CreatureTag Groups { get; set; }
        public float Weight { get; set; }
    }
}
