namespace Orikivo
{
    // specifies the type of error that occured when parsing a trigger.
    public enum TriggerParseError
    {
        TriggerCriteriaNotMet = 1,
        ArgCriteriaNotMet = 2,
        ArgValueCriteriaNotMet = 3,
        InvalidTrigger = 4,
        InvalidArg = 5,
        UserNotFound = 6,
        ArgValueNotFound = 7
    }
}
