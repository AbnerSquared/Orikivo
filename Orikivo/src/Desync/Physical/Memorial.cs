namespace Orikivo.Desync
{
    public class Memorial
    {
        /// <summary>
        /// Represents where the Husk was desynchronized.
        /// </summary>
        public Locator Location { get; set; }

        /// <summary>
        /// Represents the backpack that was left behind.
        /// </summary>
        // while the husk can always return to pick up everything, the backpack will always be damaged.
        // likewise, once the husk discovers the memorial, once they leave, everything turns to dust.
        public Backpack Backpack { get; set; }
    }
}
