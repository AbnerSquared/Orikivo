namespace Arcadia
{
    public enum UsageType
    {

        Tool = 1,

        // Durability is ignored; when used, the Expiry is applied and left there.
        Booster = 2,

        // Item can be equipped
        Equipment = 3

    }
}