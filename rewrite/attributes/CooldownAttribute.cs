using System;

namespace Orikivo
{
    // Marks a command to place a cooldown on the user executing it.
    /// <summary>
    /// An attribute that marks a command to place a cooldown on the user executing it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CooldownAttribute : Attribute
    {
        public double Seconds { get; } // Convert to TimeSpan?

        public CooldownAttribute(double seconds)
        {
            Seconds = seconds;
        }
    }
}
