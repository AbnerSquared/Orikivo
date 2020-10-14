namespace Arcadia.Models
{
    public interface IItem : IModel<string>
    {
        string GroupId { get; }

        string Icon { get; }

        string Summary { get; }

        long Tag { get; }

        int Rarity { get; }

        int Currency { get; }

        long Value { get; }

        long Size { get; }
    }
}
