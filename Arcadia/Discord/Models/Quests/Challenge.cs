namespace Arcadia
{
    // Daily challenges offer bigger rewards and take up quest slots, but smaller challenges can become available
    // Once you complete all of the daily quests
    // TODO: Remove this class and rename Quest to Challenge
    /// <summary>
    /// Represents a basic objective.
    /// </summary>
    public class Challenge
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int Difficulty { get; set; } // The higher the number, the harder it is
        // You want the difficulty to increase as the user completes more challenge sets
        // For each set completed, increase the cap of where the difficulty cuts off.

        public CriterionTriggers Triggers { get; set; }

        public Criterion Criterion { get; internal set; }
    }
}
