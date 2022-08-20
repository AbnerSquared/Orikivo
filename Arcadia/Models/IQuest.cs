namespace Arcadia.Models
{
    public interface IQuest : IModel<string>
    {
        string Summary { get; }

        int Difficulty { get; }

        int Type { get; }
    }
}
