namespace Arcadia.Old
{
    // a value specifying what an object is.
    public class GameObject
    {
        public GameObject(GameObjectType type, string value, bool isArray = false)
        {
            Type = type;
            Value = value;
            IsArray = isArray;
        }

        public GameObjectType Type { get; }
        public bool IsArray { get; } = false;
        public string Value { get; }
        // get from game data.
    }
}
