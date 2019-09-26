using System;

namespace Orikivo
{
    // Marks a command to place a cooldown on the user executing it.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CooldownAttribute : Attribute
    {
        public double Seconds { get; }

        public CooldownAttribute(double seconds)
        {
            Seconds = seconds;
        }
    }
}
