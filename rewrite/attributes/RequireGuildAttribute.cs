using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // Marks a command to only function within a guild.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireGuildAttribute : Attribute {}
}
