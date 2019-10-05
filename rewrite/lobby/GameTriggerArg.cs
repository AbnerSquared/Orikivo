namespace Orikivo
{
    public class GameTriggerArg
    {
        public GameTriggerArg(string name, GameArgType type)
        {
            Name = name;
            Type = type;
        }
        
        public string Name { get; }
        public GameArgType Type { get; }
    }
}
