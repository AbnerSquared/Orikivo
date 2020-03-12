namespace Orikivo
{
    public interface IGameCriterion<in T>
    {
        bool Check(T value);
    }
}
