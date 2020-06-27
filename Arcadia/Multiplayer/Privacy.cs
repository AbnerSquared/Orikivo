namespace Arcadia
{
    public enum Privacy
    {
        // anyone can find your game and join
        Public = 1,

        // people can only join if you give them the id
        Unlisted = 2,

        // people can only join from the guild the server was created in
        Local = 4
    }
}
