namespace Arcadia.Models
{
    public interface IModel<out TId>
    {
        TId Id { get; }

        string Name { get; }
    }

    public interface IUser : IModel<ulong>
    {
        System.DateTime CreatedAt { get; }
    }

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

    public interface IQuest : IModel<string>
    {
        string Summary { get; }

        int Difficulty { get; }

        int Type { get; }
    }

    public interface IMerit : IModel<string>
    {
        string GroupId { get; }

        int Rank { get; }
    }

    public interface IShop : IModel<string>
    {
        string Quote { get; }

        long SellTag { get; }
    }
}
