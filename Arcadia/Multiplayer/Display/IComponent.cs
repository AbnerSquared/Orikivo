namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a generic text component.
    /// </summary>
    public interface IComponent
    {
        // The identifier for this component
        string Id { get; }

        // If true, includes this component in the display content
        bool Active { get; set; }

        // If true, will automatically draw when given the chance
        bool AutoDraw { get; set; }

        // specifies the position of this component in relation to others
        int Position { get; set; }

        // specifies the component formatter used
        ComponentFormatter Formatter { get; }

        // Represents the last rendered string from this component
        string Buffer { get; }

        // Toggles the component
        public bool Toggle()
            => Active = !Active;

        string Draw();

        string Draw(params object[] args);
    }
}
