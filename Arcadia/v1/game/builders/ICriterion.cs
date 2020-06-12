namespace Arcadia.Old
{
    public interface IGameCriterion<in T>
    {
        bool Check(T value);
    }
}
