namespace Arcadia.Games
{
    public class CatanPort
    {
        public CatanResourceType Resource { get; set; }

        // Normally 3 if ANY
        // Otherwise, 2
        public int RequiredInput { get; set; }

        // The amount that is given
        // Normally, 1
        public int Output { get; set; }
    }
}