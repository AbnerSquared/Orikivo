namespace Arcadia
{
    // a single string
    public class Component : IComponent
    {
        public string Id { get; }
        
        public bool Active { get; set; }

        public int Position { get; set; }

        public ComponentFormatter Formatter { get; }

        public string Value { get; internal set; }

        public string Draw()
        {

            return "";
        }

        public void Set(string value)
        {
            Value = value;
        }
    }
}
