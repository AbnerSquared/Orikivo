namespace Orikivo
{
    public class Recipe
    {
        public string[] RequiredItemIds { get; set; }
        public string ResultId { get; set; }
        public int ResultCount { get; set; } = 1;
    }
}
