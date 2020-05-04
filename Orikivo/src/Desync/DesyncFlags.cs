namespace Orikivo.Desync
{
    public static class DesyncFlags
    {
        // lets the client know a husk was already initialized.
        public const string Initialized = "flag:initialized";

        public const string HasSpokenToNpc = "flag:has_spoken_to_npc";

        // lets the client know the door has been unlocked for sector 0.
        public const string HasUnlockedDoorSector0 = "flag:has_unlocked_door_sector_0";
    }
}
