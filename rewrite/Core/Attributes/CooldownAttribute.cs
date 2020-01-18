using System;

namespace Orikivo
{
    /// <summary>
    /// An attribute that marks a command to apply a usage cooldown on the account that executed the command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CooldownAttribute : Attribute
    {
        public TimeSpan Duration { get; }

        public CooldownAttribute(double seconds)
        {
            Duration = TimeSpan.FromSeconds(seconds);
        }

        public CooldownAttribute(TimeSpan duration)
        {
            Duration = duration;
        }
    }
}
