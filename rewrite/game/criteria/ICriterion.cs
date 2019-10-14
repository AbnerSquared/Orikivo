namespace Orikivo
{
    public interface ICriterion<in T>
    {
        bool Check(T value);
    }
}
