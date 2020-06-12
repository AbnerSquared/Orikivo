namespace Arcadia
{
    public interface IComponent
    {
        string Id { get; }

        bool Active { get; set; }

        int Position { get; set; }

        ComponentFormatter Formatter { get; }

        // represents the last rendered string from this component
        string Buffer { get; }

        public bool Toggle()
            => Active = !Active;

        string Draw();

        string Draw(params object[] args);
    }
}
