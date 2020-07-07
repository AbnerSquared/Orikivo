namespace Orikivo.Drawing.Graphics2D
{
    // This needs to be an add-on to Orikivo.Drawing.
    /// <summary>
    /// Represents a living cell that lives within a <see cref="ConwayRenderer"/>.
    /// </summary>
    public class ConwayCell
    {
        public static ConwayCell FromRandom()
        {
            bool active = RandomProvider.Instance.Next(0, 100) > 50;
            return new ConwayCell(active);
        }

        public ConwayCell Clone()
            => new ConwayCell(Active)
            {
                LastActiveTick = LastActiveTick,
                Initialized = Initialized
            };

        public ConwayCell(bool active)
        {
            Active = active;
        }

        private bool _active = false;
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;

                if (_active)
                    Initialized = true;
            }
        }

        public long LastActiveTick { get; set; } = 0;

        public bool Initialized { get; private set; } = false;
        
        public void Toggle()
            => Active = !Active;
    }
}
