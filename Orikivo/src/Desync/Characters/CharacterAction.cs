namespace Orikivo.Desync
{
    public enum CharacterAction
    {
        // the character is not doing anything
        None = 1,

        // the character is doing something; cannot interact with players
        Busy = 2,

        // the character 
        Work = 4
    }
}
