namespace Arcadia.Multiplayer
{
    public interface IPlayerData
    {
        Player Source { get; }

        void SetValue(string id, object value);

        void SetValue(string id, string fromId);

        void AddToValue(string id, int value);

        object ValueOf(string id);

        T ValueOf<T>(string id);

        void ResetProperty(string id);

        void Reset();
    }
}