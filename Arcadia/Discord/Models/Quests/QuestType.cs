namespace Arcadia
{
    public enum QuestType
    {
        // These can only be assigned once per quest assignment period.
        Daily = 1,

        // These can always be re-assigned at any point.
        Challenge = 2,

        // These are special one-time quests that give unique rewards
        // However, these are only found from items or given from shops
        Special = 3
    }
}
