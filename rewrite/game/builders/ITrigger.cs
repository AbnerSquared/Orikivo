namespace Orikivo
{
    // the generic structure for a trigger.
    public interface ITrigger<T> where T : ITriggerContext
    {
        bool CanParse(string context);
        T Parse(string context);
    }
}
