namespace Orikivo.Desync
{
    public class CharacterOutfit
    {
        // id of the outfit
        public string Id { get; set; }

        // name of the outfit.
        public string Name { get; set; }

        public AppearanceNode FaceOverlay { get; set; }

        public AppearanceNode TorsoOverlay { get; set; }
    }
}
