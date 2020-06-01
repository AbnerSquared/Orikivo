namespace Arcadia
{
    public interface IComponent
    {
        string Id { get; }
        bool Active { get; set; }

        int Position { get; set; }

        ComponentFormatter Formatter { get; }

        public bool Toggle()
            => Active = !Active;

        string Draw();
    }
}
