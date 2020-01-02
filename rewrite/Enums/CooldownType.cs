namespace Orikivo
{
    /// <summary>
    /// Defines a type of cooldown to be used or called.
    /// </summary>
    [System.Flags]
    public enum CooldownType
    {
        Internal = Command | Global | Notify,
        
        Storeable = Claimable | Item,

        Claimable = 2,

        Item = 4,

        Command = 8,

        Global = 16,

        Notify = 32
    }
}
